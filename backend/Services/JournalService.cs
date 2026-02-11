using JournalApi.Data;
using JournalApi.Models;
using JournalApi.Models.DTOs;
using Microsoft.EntityFrameworkCore;
using JournalApi.Models.Common;

public class JournalService : IJournalService
{
    private readonly JournalDbContext _db;
    
    public JournalService(JournalDbContext db)
    {
        _db = db;
    }
    private JournalDto MapToDto(JournalEntry journal)
    {
        return new JournalDto
        {
            Id = journal.Id,
            Title = journal.Title,
            Content = journal.Content,
            CreatedAt = journal.CreatedAt,
            Tags = journal.JournalTags
                .Select(jt => new TagDto
                {
                    Id = jt.TagId,
                    Name = jt.Tag.Name
                }).ToList(),
            Categories = journal.JournalCategories
                .Select(jc => new CategoryDto
                {
                    Id = jc.CategoryId,
                    Name = jc.Category.Name
                }).ToList()
        };
    } 
    public async Task<PagedResult<JournalDto>> GetAllAsync(
        string userId, 
        int sort,
        int page, 
        int pageSize,
        int? tagId = null,
        int? categoryId = null,
        string? search = null)
    {
        var query = _db.JournalEntries
            .Include(j => j.JournalTags)
                .ThenInclude(jt => jt.Tag)
            .Include(j => j.JournalCategories)
                .ThenInclude(jc => jc.Category)
            .Where (j => j.UserId == userId && !j.IsDeleted);


        if (!string.IsNullOrWhiteSpace(search))
        {
            var lowerSearch = search.ToLower();
            query = query.Where(j =>
                EF.Functions.Like(j.Title.ToLower(), $"%{lowerSearch}%") ||
                EF.Functions.Like(j.Content.ToLower(), $"%{lowerSearch}%"));
        }

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
            TotalCount = total  
        };

    }

    public async Task<JournalDto?> GetByIdAsync(int id, string userId)
    {
        var entry = await _db.JournalEntries
            .Include(j => j.JournalTags).ThenInclude(jt => jt.Tag)
            .Include(j => j.JournalCategories).ThenInclude(jc => jc.Category)
            .FirstOrDefaultAsync(j => j.Id == id && j.UserId == userId && !j.IsDeleted);

            return entry == null ? null : MapToDto(entry);
    
    }

    public async Task<JournalDto> CreateAsync(
        CreateJournalDto dto,
        string userId)
    {
        var journal = new JournalEntry
        {
            Title = dto.Title,
            Content = dto.Content,
            UserId = userId,
            CreatedAt = DateTime.UtcNow
        };

        foreach (var categoryId in dto.CategoryIds.Distinct())
        {
            journal.JournalCategories.Add(new JournalCategory
            {
                CategoryId = categoryId
            });
        }

        foreach (var tagId in dto.TagIds.Distinct())
        {
        
            journal.JournalTags.Add(new JournalTag
            {
                TagId = tagId
            });
        }
    
        _db.JournalEntries.Add(journal);
        await _db.SaveChangesAsync();

        return MapToDto(journal);
    }


    public async Task<JournalDto?> UpdateAsync(
        int id, 
        UpdateJournalDto dto,
        string userId)
    {
        var journal = await _db.JournalEntries
            .Include(j => j.JournalTags)
            .Include(j => j.JournalCategories)
            .FirstOrDefaultAsync(j => j.Id == id && j.UserId == userId);

        if (journal == null) return null;

        journal.Title = dto.Title;
        journal.Content = dto.Content;

        journal.JournalCategories.Clear();
        foreach (var categoryId in dto.CategoryIds.Distinct())
        {
            journal.JournalCategories.Add(new JournalCategory { CategoryId = categoryId });
        }

        journal.JournalTags.Clear();
        foreach (var tagId in dto.TagIds.Distinct())
        {
            journal.JournalTags.Add(new JournalTag { TagId = tagId });
        }

        await _db.SaveChangesAsync();
        return MapToDto(journal);
    }

    public async Task<int> GetCountAsync(string userId, string? search = null)
{
    var query = _db.JournalEntries.Where(j => j.UserId == userId && !j.IsDeleted);

    if (!string.IsNullOrWhiteSpace(search))
    {
        var lowerSearch = search.ToLower();
        query = query.Where(j =>
            EF.Functions.Like(j.Title.ToLower(), $"%{lowerSearch}%") ||
            EF.Functions.Like(j.Content.ToLower(), $"%{lowerSearch}%"));
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

