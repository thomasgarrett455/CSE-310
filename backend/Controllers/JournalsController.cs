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
    private readonly IJournalService _service;

    public JournalsController(IJournalService service)
    {
        _service = service;
    }

    // GET: api/journals
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _service.GetAllAsync());
    }

    // GET: api/journals/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        var entry = await _service.GetByIdAsync(id);
        if (entry == null)
            return NotFound(new { message = "Journal entry not found" });

        return Ok(entry);
    }

    // POST: api/journals
    [HttpPost]
    public async Task<IActionResult> Create(CreateJournalDto dto)
    {
        // Model validation automatically runs due to [ApiController]
        var entry = await _service.CreateAsync(dto.Title, dto.Content);
return CreatedAtAction(nameof(Get), new { id = entry.Id }, entry);
    }

    // PUT: api/journals/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, UpdateJournalDto dto)
    {
        var entry = await _service.UpdateAsync(id, dto.Title, dto.Content);
        if (entry == null)
            return NotFound(new { message = "Journal entry not found" });
        return Ok(entry);
    }

    // DELETE: api/journals/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _service.DeleteAsync(id);
        if (!deleted)
            return NotFound(new { message = "Journal entry not found" });
        return NoContent();
    }
}
