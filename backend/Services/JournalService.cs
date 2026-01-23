using JournalApi.Data;
using JournalApi.Models;
using Microsoft.EntityFrameworkCore;

public class JournalService : IJournalService
{
  private readonly JournalDbContext _db;

  public JournalService(JournalDbContext db)
  {
    _db = db;
  }

  public async Task<List<JournalEntry>> GetAllAsync()
  {
    return await _db.JournalEntries.ToListAsync();
  }


  public async Task<JournalEntry?> GetByIdAsync(int id)
  {
    return await _db.JournalEntries.FindAsync(id);
  }

  public async Task<JournalEntry> CreateAsync(string title, string content)
  {
    var entry = new JournalEntry
    {
      Title = title,
      Content = content,
      CreatedAt = DateTime.UtcNow
    };

    _db.JournalEntries.Add(entry);
    await _db.SaveChangesAsync();

    return entry;
  }

    public async Task<JournalEntry?> UpdateAsync(int id, string title, string content)
    {
        var entry = await _db.JournalEntries.FindAsync(id);
        if (entry == null) return null;

        entry.Title = title;
        entry.Content = content;

        await _db.SaveChangesAsync();
        return entry;
    }

  public async Task<bool> DeleteAsync(int id)
    {
        var entry = await _db.JournalEntries.FindAsync(id);
        if (entry == null) return false;

        _db.JournalEntries.Remove(entry);
        await _db.SaveChangesAsync();
        return true;
    }


}