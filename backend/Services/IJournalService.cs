using JournalApi.Models;
using JournalApi.Models.DTOs;
using JournalApi.Models.Common;

public interface IJournalService
{
  Task<PagedResult<JournalDto>> GetAllAsync(
    string userId, 
    int sort,
    int page, 
    int pageSize,
    int? tagIds = null,
    int? categoryId = null,
    string? search = null);
  Task<JournalDto?> GetByIdAsync(int id, string userId);
  Task<JournalDto> CreateAsync(CreateJournalDto journalDto, string userId);
  Task<JournalDto?> UpdateAsync(int id, UpdateJournalDto journalDto, string userId);
  Task<int> GetCountAsync(string userId, string? search);
  Task<bool> DeleteAsync(int id, string userId);
  Task<bool>RestoreAsync(int id, string userId);
  Task<List<JournalDto>> GetDeletedAsync(string userId);

}