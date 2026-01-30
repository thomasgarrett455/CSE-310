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
    public async Task<IActionResult> GetAll([FromQuery] int page =  1, [FromQuery] int pageSize = 10)
    {
        var userId = GetUserId();
        var entries = await _service.GetAllAsync(userId, page, pageSize);
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
    public async Task<IActionResult> Create(CreateJournalDto dto)
    {
        var userId = GetUserId();
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        // Model validation automatically runs due to [ApiController]
        var entry = await _service.CreateAsync(dto.Title, dto.Content, userId);
        return CreatedAtAction(nameof(Get), new { id = entry.Id }, entry);
    }

    // PUT: api/journals/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, UpdateJournalDto dto, string userId)
    {

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        var entry = await _service.UpdateAsync(id, dto.Title, dto.Content, userId);
        if (entry == null)
            return NotFound(new { message = "Journal entry not found" });
        return Ok(entry);
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
}
