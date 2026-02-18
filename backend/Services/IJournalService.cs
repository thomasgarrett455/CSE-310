using JournalApi.Models.Common;
using JournalApi.Models.DTOs;

namespace JournalApi.Services;

public interface IJournalService
{
    // Basic CRUD
    Task<PagedResult<JournalDto>> GetAllAsync(string userId, JournalSort sort, int page, int pageSize, int? tagId = null, int? categoryId = null, string? search = null);
    Task<JournalDto?> GetByIdAsync(int id, string userId);
    Task<JournalDto> CreateAsync(CreateJournalDto dto, string userId);
    Task<JournalDto?> UpdateAsync(int id, UpdateJournalDto dto, string userId);

    Task<int> GetCountAsync(string userId, string? search = null);
    Task<bool> SoftDeleteAsync(int id, string userId);
    Task<bool> RestoreAsync(int id, string userId);

    // Analytics
    Task<int> GetTotalJournalsAsync(string userId);
    Task<int> GetDeletedCountAsync(string userId);
Task<List<CountDto>> GetMostUsedTagsAsync(string userId, int top = 5);
    Task<List<CountDto>> GetMostUsedCategoriesAsync(string userId, int top = 5);
    Task<List<JournalAnalyticsDto>> GetMonthlyJournalCountsAsync(string userId, int year);
    Task<List<JournalAnalyticsDto>> GetYearlyJournalCountsAsync(string userId);

    Task<List<JournalTimeAnalyticsDto>> GetJournalsByDayOfWeekAsync(string userId);

    Task<List<JournalTimeAnalyticsDto>> GetJournalsByHourOfDayAsync(string userId);
}
