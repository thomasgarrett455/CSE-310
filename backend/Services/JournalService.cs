namespace JournalApi.Services;

using JournalApi.Data;
using JournalApi.Models;
using JournalApi.Models.DTOs;
using Microsoft.EntityFrameworkCore;
using JournalApi.Models.Common;

public class JournalService : IJournalService

{
    private readonly JournalDbContext _db;
    private readonly ILogger<JournalService> _logger;
    
    public JournalService(JournalDbContext db, ILogger<JournalService> logger)
    {
        _db = db;
        _logger = logger;

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
        JournalSort sort,
        int page, 
        int pageSize,
        int? tagId = null,
        int? categoryId = null,
        string? search = null)
    {
        try
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

            query = sort switch
            {
                JournalSort.CreatedAtAsc => query.OrderBy(j => j.CreatedAt),
                JournalSort.TitleAsc => query.OrderBy(j => j.Title),
                JournalSort.TitleDesc => query.OrderByDescending(j => j.Title),
                _ => query.OrderByDescending(j => j.CreatedAt)
            };

            var total = await query.CountAsync();

            var journals = await query
                .OrderByDescending(j => j.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<JournalDto>
            {
                Items = journals.Select(MapToDto).ToList(),
                TotalCount = total,
                Page = page,
                PageSize = pageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching journals for user {UserId}", userId);
            throw new ApplicationException("Failed to get journals. Please try again.");
        }

    }

    public async Task<JournalDto?> GetByIdAsync(int id, string userId)
    {
        try
        {
            var entry = await _db.JournalEntries
                .Include(j => j.JournalTags).ThenInclude(jt => jt.Tag)
                .Include(j => j.JournalCategories).ThenInclude(jc => jc.Category)
                .FirstOrDefaultAsync(j => j.Id == id && j.UserId == userId && !j.IsDeleted);

                return entry == null ? null : MapToDto(entry);
        
        } catch (Exception ex) 
        {
            _logger.LogError(ex, "Error fetching journal {JournalId} for user {UserId}", id, userId);
            throw;
        }
    }


        public async Task<JournalDto> CreateAsync(
            CreateJournalDto dto,
            string userId)
        {
            try
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
                
                _logger.LogInformation("Created journal {JournalId} for user {UserId}", journal.Id, userId);
                return MapToDto(journal);
            } 
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Error creating journal for user {UserId}", userId);
                throw new ApplicationException("Could not create journal entry. Please try again.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error creating journal for user {UserId}", userId);
                throw;
            }
        
    }


    public async Task<JournalDto?> UpdateAsync(
        int id, 
        UpdateJournalDto dto,
        string userId)
    {
        try
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
            _logger.LogInformation("Updated journal {JournalId} for user {UserId}", id, userId);

            return MapToDto(journal);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating journal {JournalId} for user {UserId}", id, userId);
            throw;
        }
    }

    

    public async Task<int> GetCountAsync(string userId, string? search = null)
{
        try
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
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error counting journals for user {UserId}", userId);
            throw;
        }
}

    public async Task<bool> SoftDeleteAsync(int id, string userId)
    {
        try
        {
            
        var entry = await _db.JournalEntries
            .FirstOrDefaultAsync(j => j.Id == id && j.UserId == userId);
        if (entry == null) return false;

        entry.IsDeleted = true;
        entry.DeletedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        _logger.LogInformation("Soft deleted journal {JournalId} for user {UserId}", id, userId);
        return true;
    }
    catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting journal {JournalId} for user {UserId}", id, userId);
            throw;
        }
    }

    public async Task<bool> RestoreAsync(int id, string userId)
    {
            try
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
            _logger.LogInformation("Restored journal {JournalId} for user {UserId}", id, userId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error restoring journal {JournalId} for user {UserId}", id, userId);
            throw;
        }
    }
    public async Task<List<JournalDto>> GetDeletedAsync(string userId)
    {
        try
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
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching deleted journal for user {UserId}", userId);
            throw;
        }
    }

    public async Task<int> GetTotalJournalsAsync(string userId)
    {
        try
        {
        return await _db.JournalEntries
            .Where(j => j.UserId == userId && !j.IsDeleted)
            .CountAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error counting total journals for user {UserId}", userId);
            throw;
        }
    }

// Most used tags
public async Task<List<CountDto>> GetMostUsedTagsAsync(string userId, int top = 5)
{
    try
        {
        return await _db.JournalTags
            .Where(jt => jt.JournalEntry.UserId == userId && !jt.JournalEntry.IsDeleted)
            .GroupBy(jt => jt.Tag.Name)
            .Select(g => new CountDto
            { 
                Name = g.Key, 
                Count = g.Count() 
                })
            .OrderByDescending(x => x.Count)
            .Take(top)
            .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching top tags for user {UserId}", userId);
            throw;
        }
}

    public async Task<List<CountDto>> GetMostUsedCategoriesAsync(string userId, int top = 5)
    {
        try
        {
        return await _db.JournalCategories
            .Where(jc => jc.JournalEntry.UserId == userId && !jc.JournalEntry.IsDeleted)
            .GroupBy(jc => jc.Category.Name)
            .Select(g => new CountDto
            {
                Name = g.Key,
                Count = g.Count()
            })
            .OrderByDescending(x => x.Count)
            .Take(top)
            .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching top categories for user {UserId}", userId);
            throw;
        }
    }

    public async Task<int> GetDeletedCountAsync(string userId)
    {
        try
        {
        return await _db.JournalEntries
            .Where(j => j.UserId == userId && j.IsDeleted)
            .CountAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error counting deleted journals for user {UserId}", userId);
            throw;
        }
    }

    public async Task<List<JournalAnalyticsDto>> GetMonthlyJournalCountsAsync(string userId, int year)
    {
        try
        {
            return await _db.JournalEntries
                .Where(j => j.UserId == userId && !j.IsDeleted && j.CreatedAt.Year == year)
                .GroupBy(j => j.CreatedAt.Month)
                .Select(g => new JournalAnalyticsDto
                {
                    Period = g.Key.ToString("D2"),
                    Count = g.Count()
                })
                .OrderBy(x => x.Period)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching monthly journal counts for user {UserId}, year {Year}", userId, year);
            throw new ApplicationException("Failed to get monthly journal analytics.");
        }
    }

    public async Task<List<JournalAnalyticsDto>> GetYearlyJournalCountsAsync(string userId)
    {
        try
        {
        return await _db.JournalEntries
            .Where(j => j.UserId == userId && !j.IsDeleted)
            .GroupBy(j => j.CreatedAt.Year)
            .Select(g => new JournalAnalyticsDto
            {
                Period = g.Key.ToString(),
                Count = g.Count()
            })
            .OrderBy(x => x.Period)
            .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching yearly journal counts for user {UserId}", userId);
            throw new ApplicationException("Failed to get yearly journal analytics.");
        }
    }


    public async Task<List<JournalTimeAnalyticsDto>> GetJournalsByDayOfWeekAsync(string userId)
    {
        try
        {
            return await _db.JournalEntries
                .Where(j => j.UserId == userId && !j.IsDeleted)
                .GroupBy(j => j.CreatedAt.DayOfWeek)
                .Select(g => new JournalTimeAnalyticsDto
                {
                    Label = g.Key.ToString(),
                    Count = g.Count()
                })
                .OrderBy(g => g.Label)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching journal counts by day of week for user {UserId}", userId);
            throw new ApplicationException("Failed to get journals by day of week.");
        }
    }

    public async Task<List<JournalTimeAnalyticsDto>> GetJournalsByHourOfDayAsync(string userId)
    {
        try
        {
            return await _db.JournalEntries 
                .Where(j => j.UserId == userId && !j.IsDeleted)
                .GroupBy(j => j.CreatedAt.Hour)
                .Select(g => new JournalTimeAnalyticsDto
                {
                    Label = g.Key.ToString("D2"),
                    Count = g.Count()
                })
                .OrderBy(g => g.Label)
                .ToListAsync(); 
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching journal counts by hour of day for user {UserId}", userId);
            throw new ApplicationException("Failed to get journals by hour of day.");
        }
    }

    public async Task<bool> ToggleSaveAsync(int journalId, string userId)
    {
        var journal = await _db.JournalEntries
            .FirstOrDefaultAsync(j => j.Id == journalId && j.UserId == userId);
        
        if (journal == null)
            return false;
        
        journal.IsSaved = !journal.IsSaved;
        await _db.SaveChangesAsync();

        return true;
    }

    public async Task<List<JournalEntry>> GetSavedAsync(string userId)
    {
        return await _db.JournalEntries
            .Where(j => j.UserId == userId && j.IsSaved && !j.IsDeleted)
            .ToListAsync();
    }

}


