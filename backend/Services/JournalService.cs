using JournalApi.Data;
using JournalApi.Models;
using JournalApi.Models.DTOs;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using JournalApi.Models.Common;

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

    public async Task<PagedResult<JournalDto>> GetAllAsync(
        string userId, 
        int page, 
        int pageSize,
        int? tagId = null,
        int? categoryId = null)
    {
        var query = _db.JournalEntries
            .Where (j => j.UserId == userId && !j.IsDeleted);

        if (tagId.HasValue)
        {
            query = query.Where(j => j.JournalTags.Any(t => t.TagId == tagId));
        }

        if (categoryId.HasValue)
        {
            query = query.Where(j => j.JournalCategories.Any(c => c.CategoryId == categoryId));
        }

        var total = await query.CountAsync();
        var journals = await query
            .OrderByDescending(j => j.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<JournalDto>
        {
            Items = journals.Select(MapToDto).ToList(),
            totalCount = total  
        };

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
        List<int> tagIds,
        List<int> categoryIds,
        string userId)
    {
        var journal = new JournalEntry
        {
            Title = title,
            Content = content,
            UserId = userId,
            CreatedAt = DateTime.UtcNow
        };

        foreach (var tagId in tagIds)
        {
            journal.JournalTags.Add(new JournalTag
            {
                TagId = tagId
            });
        }

        foreach (var catgegoryId in categoryIds)
        {
            journal.JournalCategories.Add(new JournalCategory
            {
                CategoryId = catgegoryId
            });
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

