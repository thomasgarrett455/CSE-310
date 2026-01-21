using Microsoft.AspNetCore.Mvc;
using JournalApi.Models;
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

  [HttpGet]
  public IActionResult GetAll()
  {
    return Ok(_db.JournalEntries.ToList());
  }

  [HttpGet("{id}")]
  public IActionResult Get(int id)
  {
    var entry = _db.JournalEntries.Find(id);
    if (entry == null)
        return NotFound();
      
    return Ok(entry);
  }

  [HttpPost]
  public IActionResult Create(JournalEntry entry)
  {
    _db.JournalEntries.Add(entry);
    _db.SaveChanges();

    return CreatedAtAction(nameof(Get), new { id = entry.Id }, entry);
  }

  [HttpPut("{id}")]
  public IActionResult Update(int id, JournalEntry updated)
  {
    var entry = _db.JournalEntries.Find(id);
    if (entry == null)
        return NotFound();
      
    entry.Title = updated.Title;
    entry.Content = updated.Content;

    _db.SaveChanges();
    return Ok(entry);
  }

  [HttpDelete("{id}")]
  public IActionResult Delete(int id)
  {
    var entry = _db.JournalEntries.Find(id);
    if (entry == null) 
        return NotFound();
      
    _db.JournalEntries.Remove(entry);
    _db.SaveChanges();

    return NoContent();
  }

}