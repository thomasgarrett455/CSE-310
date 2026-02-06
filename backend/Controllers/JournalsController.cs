using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using JournalApi.Models;
using JournalApi.Models.DTOs;
using JournalApi.Data;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.AspNetCore.Authorization;
namespace JournalApi.Controllers;
using System.Security.Claims;

 
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
        [FromQuery] string sort = "created_desc",
        [FromQuery] int page =  1, 
        [FromQuery] int pageSize = 10,
        [FromQuery] int? categoryId = null,
        [FromQuery] List<int>? tagIds = null)
            {
        var userId = GetUserId();
        var entries = await _service.GetAllAsync(
            userId, 
            search, 
            sort,
            page, 
            pageSize,
            categoryId,
            tagIds);
        return Ok(entries);
    }

    // GET: api/journals/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id, string userId)
    {
        var entry = await _service.GetByIdAsync(id, userId);
        if (entry == null)
            return NotFound(new { message = "Journal entry not found" });

        return Ok(entry);
    }

    // POST: api/journals
    [HttpPost]
    public async Task<IActionResult> Create(CreateJournalWithMetaDto dto)
    {
        var userId = GetUserId();
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        // Model validation automatically runs due to [ApiController]
        var journal = await _service.CreateAsync(dto.Title, dto.Content, dto.CategoryId, dto.Tags, userId);
        return CreatedAtAction(nameof(Get), new { id = journal.Id }, journal);
    }

    // PUT: api/journals/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, UpdateJournalWithMetaDto dto)
    {
        var userId = GetUserId();

        var journal = await _service.UpdateAsync(id, dto.Title, dto.Content, dto.CategoryId, dto.Tags, userId);
        if (journal == null)
            return NotFound(new { message = "Journal entry not found" });
        return Ok(journal);
    }

    [HttpGet("count")]
    public async Task<IActionResult> GetCount([FromQuery] string? search)
    {
        var userId = GetUserId();
        var count = await _service.GetCountAsync(userId, search);
        return Ok(new { total = count });
    }

    // DELETE: api/journals/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id, string userId)
    {
        var deleted = await _service.DeleteAsync(id, userId);
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
        return Ok(await _service.GetDeletedAsync(userId));
    }
}
