using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using JournalApi.Models;
using JournalApi.Models.DTOs;
using JournalApi.Services;
using JournalApi.Data;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using JournalApi.Models.Common;
namespace JournalApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/journals")]
    public class JournalsController : ControllerBase
    {
        private string GetUserId() =>
            User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        private readonly IJournalService _service;
        public JournalsController(IJournalService service)
        {
            _service = service;
        }

        // GET: api/journals
        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] string? search,
            [FromQuery] JournalSort sort = JournalSort.CreatedAtDesc,
            [FromQuery] int page =  1, 
            [FromQuery] int pageSize = 10,
            [FromQuery] int? categoryId = null,
            [FromQuery] int? tagId = null)
                {
            var userId = GetUserId();
            var entries = await _service.GetAllAsync(
                userId,
                sort, 
                page, 
                pageSize,
                tagId,
                categoryId,
                search
            );
            
            return Ok(entries);
        }

        // GET: api/journals/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var userId = GetUserId();
            var entry = await _service.GetByIdAsync(id, userId);
            if (entry == null)
                return NotFound(new { message = "Journal entry not found" });

            return Ok(entry);
        }

        // POST: api/journals
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateJournalDto dto)
        {
            var userId = GetUserId();

            var journal = await _service.CreateAsync(dto, userId);

            return CreatedAtAction(nameof(Get), new { id = journal.Id}, journal);
        }

        // PUT: api/journals/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateJournalDto dto)
        {
            var userId = GetUserId();

            var journal = await _service.UpdateAsync(id, dto, userId);

            if (journal == null)
                return NotFound(new { message = "Journal entry not found" });

            return Ok(journal);
        }

        [HttpGet("count")]
        public async Task<IActionResult> GetCount([FromQuery] string? search = null)
        {
            var userId = GetUserId();
            var totalJournals = await _service.GetTotalJournalsAsync(userId);
            int count;
            if (!string.IsNullOrWhiteSpace(search))
            {
                
            count = await _service.GetCountAsync(userId, search);
            } else
            {
                count = totalJournals;
            }

            var deletedCount = await _service.GetDeletedCountAsync(userId);
            
            return Ok(new { total = count, deleted = deletedCount });
        }

        // DELETE: api/journals/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = GetUserId();
            var deleted = await _service.SoftDeleteAsync(id, userId);
            if (!deleted)
                return NotFound(new { message = "Journal entry not found" });
            return NoContent();
        }

        [HttpPost("{id}/restore")]
        public async Task<IActionResult> Restore(int id)
        {
            var userId = GetUserId();
            var restored = await _service.RestoreAsync(id, userId);

            if (!restored) 
                return NotFound(new { message = "Journal not found" });

            return NoContent();
        }

        [HttpGet("trash")]
        public async Task<ActionResult> GetTrash()
        {
            var userId = GetUserId();
            var trashed = await _service.GetDeletedCountAsync(userId);

            return Ok(trashed);
        }

        [HttpGet("analytics/summary")]
        public async Task<IActionResult> GetAnalytics()
        {
            var userId = GetUserId();

            var totalJournals = await _service.GetTotalJournalsAsync(userId);
            var deletedJournals = await _service.GetDeletedCountAsync(userId);
            var topTags = await _service.GetMostUsedTagsAsync(userId);
            var topCategories = await _service.GetMostUsedCategoriesAsync(userId);

            return Ok(new
            {
                TotalJournals = totalJournals,
                DeletedJournals = deletedJournals,
                TopTags = topTags,
                TopCategories = topCategories
            });
        }

        [HttpGet("analytics/monthly")]
        public async Task<IActionResult> GetMonthlyCount([FromQuery] int year)
        {
            var userId = GetUserId();
            var data = await _service.GetMonthlyJournalCountsAsync(userId, year);
            return Ok(data);
        }

        [HttpGet("analytics/yearly")]
        public async Task<IActionResult> GetYearlyCounts()
        {
            var userId = GetUserId();
            var data = await _service.GetYearlyJournalCountsAsync(userId);
            return Ok(data);
        }

        [HttpGet("analytics/dow")]
        public async Task<IActionResult> GetJournalsByDayOfWeek()
        {
            var userId = GetUserId();
            var data = await _service.GetJournalsByDayOfWeekAsync(userId);
            return Ok(data);
        }

        [HttpGet("analytics/hourly")]
        public async Task<IActionResult> GetJournalsByHourOfDay()
        {
            var userId = GetUserId();
            var data = await _service.GetJournalsByHourOfDayAsync(userId);
            return Ok(data);
        }

        [HttpPatch("{id}/save")]
        public async Task<IActionResult> ToggleSave(int id)
        {
            var userId = GetUserId();

            var success = await _service.ToggleSaveAsync(id, userId);
            
            if (!success)
                return NotFound();

            return NoContent();
        }

        [HttpGet("saved")]
        public async Task<IActionResult> GetSaved()
        {
            var userId = GetUserId();
            var saved = await _service.GetSavedAsync(userId);
            return Ok(saved);
        }
    }

}