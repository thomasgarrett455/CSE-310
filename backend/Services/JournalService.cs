using JournalApi.Data;
using JournalApi.Models;
using JournalApi.Models.DTOs;
using Microsoft.EntityFrameworkCore;

public class JournalService : IJournalService
{
    private readonly JournalDbContext _db;
    
    private JournalDto MapToDto(JournalEntry j)
    {
        return new JournalDto
        {
            Id = j.Id,
            Title = j.Title,
            Content = j.Content,
            CreatedAt = j.CreatedAt
        };
    } 

    public JournalService(JournalDbContext db)
    {
        _db = db;
    }

    public async Task<List<JournalDto>> GetAllAsync(
        string userId, 
        string? search,
        string sort, 
        int page = 1, 
        int pageSize = 10)
    {
        IQueryable<JournalEntry> query = _db.JournalEntries
            .Where(j => j.UserId == userId && !j.IsDeleted);

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(j =>
                j.Title.Contains(search) ||
                j.Content.Contains(search) &&
                !j.IsDeleted);
        }

        query = sort.ToLower() switch
        {
            "title_asc" => query.OrderBy(j => j.Title),
            "title_desc" => query.OrderByDescending(j => j.Title),
            "created_asc" => query.OrderBy(j => j.CreatedAt),
            _ => query.OrderByDescending(j => j.CreatedAt)
        };

        return await query
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
            .FirstOrDefaultAsync(j => j.Id == id && j.UserId == userId && !j.IsDeleted);
        if (entry == null) return null;

        return new JournalDto
        {
            Id = entry.Id,
            Title = entry.Title,
            Content = entry.Content,
            CreatedAt = entry.CreatedAt
        };
    }

    public async Task<JournalDto> CreateAsync(
        string title, 
        string content, 
        int? categoryId,
        List<string> tags,
        string userId)
    {
        var journal = new JournalEntry
        {
            Title = title,
            Content = content,
            CategoryId = categoryId,
            UserId = userId,
            CreatedAt = DateTime.UtcNow
        };

        foreach (var tagName in tags.Distinct())
        {
            var existingTag = await _db.Tags
                .FirstOrDefaultAsync(t => 
                    t.Name == tagName &&
                    t.UserId == userId);
            
            if (existingTag == null)
            {
                existingTag = new Tag
                {
                    Name = tagName,
                    UserId = userId
                };

                _db.Tags.Add(existingTag);
            }

            journal.Tags.Add(existingTag);
        }

        _db.JournalEntries.Add(journal);
        await _db.SaveChangesAsync();

        return MapToDto(journal);
    }


    public async Task<JournalDto?> UpdateAsync(
        int id, 
        string title, 
        string content,
        int? categoryId,
        List<string> tags, 
        string userId)
    {
        var journal = await _db.JournalEntries
            .FirstOrDefaultAsync(j => j.Id == id && j.UserId == userId);

        if (journal == null) return null;

        journal.Title = title;
        journal.Content = content;
        journal.CategoryId = categoryId;

        journal.Tags.Clear();

        foreach (var tagName in tags.Distinct())
        {
            var existingTag = await _db.Tags
                .FirstOrDefaultAsync(t =>
                    t.Name == tagName &&
                    t.UserId == userId);
            
            if (existingTag == null)
            {
                existingTag = new Tag
                {
                    Name = tagName,
                    UserId = userId
                };

                _db.Tags.Add(existingTag);
            }

            journal.Tags.Add(existingTag);
        }

        await _db.SaveChangesAsync();
        return MapToDto(journal);
    
    }

    public async Task<int> GetCountAsync(string userId, string? search)
    {
        IQueryable<JournalEntry> query = _db.JournalEntries.Where(j => j.UserId == userId && !j.IsDeleted);

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(j =>
                j.Title.Contains(search) ||
                j.Content.Contains(search));
        }

        return await query.CountAsync();
    }

    public async Task<bool> DeleteAsync(int id, string userId)
    {
        var entry = await _db.JournalEntries
            .FirstOrDefaultAsync(j => j.Id == id && j.UserId == userId);

        if (entry == null) return false;

        entry.IsDeleted = true;
        entry.DeletedAt = DateTime.UtcNow;

        _db.JournalEntries.Remove(entry);
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> RestoreAsync(int id, string userId)
    {
        var entry = await _db.JournalEntries
            .FirstOrDefaultAsync(j => 
                j.Id == id &&
                j.UserId == userId &&
                j.IsDeleted);

        if (entry == null) return false;

        entry.IsDeleted = false;
        entry.DeletedAt = null;

        await _db.SaveChangesAsync();
        return true;
    }
    public async Task<List<JournalDto>> GetDeletedAsync(string userId)
    {
        return await _db.JournalEntries
            .Where(j => j.UserId == userId && j.IsDeleted)
            .OrderByDescending(j => j.DeletedAt)
            .Select(j => new JournalDto
            {
               Id = j.Id,
               Title = j.Title,
                Content = j.Content,
                CreatedAt = j.CreatedAt
            })
            .ToListAsync();
    }
}

