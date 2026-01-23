using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using JournalApi.Models;
using JournalApi.Models.DTOs;
using JournalApi.Data;

namespace JournalApi.Controllers;

[ApiController]
[Route("api/journals")]
public class JournalsController : ControllerBase
{
    private readonly JournalDbContext _db;

    public JournalsController(JournalDbContext db)
    {
        _db = db;
    }

    // GET: api/journals
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var journals = await _db.JournalEntries.ToListAsync();
        return Ok(journals);
    }

    // GET: api/journals/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        var entry = await _db.JournalEntries.FindAsync(id);
        if (entry == null)
            return NotFound(new { message = "Journal entry not found" });

        return Ok(entry);
    }

    // POST: api/journals
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateJournalDto dto)
    {
        // Model validation automatically runs due to [ApiController]
        var entry = new JournalEntry
        {
            Title = dto.Title,
            Content = dto.Content,
            CreatedAt = DateTime.UtcNow
        };

        _db.JournalEntries.Add(entry);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(Get), new { id = entry.Id }, entry);
    }

    // PUT: api/journals/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateJournalDto dto)
    {
        var entry = await _db.JournalEntries.FindAsync(id);
        if (entry == null)
            return NotFound(new { message = "Journal entry not found" });

        entry.Title = dto.Title;
        entry.Content = dto.Content;

        await _db.SaveChangesAsync();

        return Ok(entry);
    }

    // DELETE: api/journals/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var entry = await _db.JournalEntries.FindAsync(id);
        if (entry == null)
            return NotFound(new { message = "Journal entry not found" });

        _db.JournalEntries.Remove(entry);
        await _db.SaveChangesAsync();

        return NoContent();
    }
}
