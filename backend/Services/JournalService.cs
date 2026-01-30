using JournalApi.Data;
using JournalApi.Models;
using JournalApi.Models.DTOs;
using Microsoft.EntityFrameworkCore;

public class JournalService : IJournalService
{
    private readonly JournalDbContext _db;

    public JournalService(JournalDbContext db)
    {
        _db = db;
    }

    public async Task<List<JournalDto>> GetAllAsync(string userId, int page = 1, int pageSize = 10)
    {
        return await _db.JournalEntries
            .Where(j => j.UserId == userId)
            .OrderByDescending(j => j.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(j => new JournalDto
            {
                Id = j.Id,
                Title = j.Title,
                Content = j.Content,
                CreatedAt = j.CreatedAt
            })
            .ToListAsync();
    }

    public async Task<JournalDto?> GetByIdAsync(int id, string userId)
    {
        var entry = await _db.JournalEntries
            .FirstOrDefaultAsync(j => j.Id == id && j.UserId == userId);
        if (entry == null) return null;

        return new JournalDto
        {
            Id = entry.Id,
            Title = entry.Title,
            Content = entry.Content,
            CreatedAt = entry.CreatedAt
        };
    }

    public async Task<JournalDto> CreateAsync(string title, string content, string userId)
    {
        var entry = new JournalEntry
        {
            Title = title,
            Content = content,
            UserId = userId,
            CreatedAt = DateTime.UtcNow
        };

        _db.JournalEntries.Add(entry);
        await _db.SaveChangesAsync();

        return new JournalDto
        {
            Id = entry.Id,
            Title = entry.Title,
            Content = entry.Content,
            CreatedAt = entry.CreatedAt
        };
    }

    public async Task<JournalDto?> UpdateAsync(int id, string title, string content, string userId)
    {
        var entry = await _db.JournalEntries
            .FirstOrDefaultAsync(j => j.Id == id && j.UserId == userId);
            
        if (entry == null) return null;

        entry.Title = title;
        entry.Content = content;

        await _db.SaveChangesAsync();

        return new JournalDto
        {
            Id = entry.Id,
            Title = entry.Title,
            Content = entry.Content,
            CreatedAt = entry.CreatedAt
        };
    }

    public async Task<bool> DeleteAsync(int id, string userId)
    {
        var entry = await _db.JournalEntries
            .FirstOrDefaultAsync(j => j.Id == id && j.UserId == userId);
        if (entry == null) return false;

        _db.JournalEntries.Remove(entry);
        await _db.SaveChangesAsync();
        return true;
    }
}
