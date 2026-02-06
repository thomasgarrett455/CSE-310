using JournalApi.Models;
using JournalApi.Models.DTOs;
using JournalApi.Models.Common;

public interface IJournalService
{
  Task<PagedResult<JournalDto>> GetAllAsync(
    string userId, 
    int page, 
    int pageSize,
    int? tagId = null,
    int? categoryId = null);
  Task<JournalDto?> GetByIdAsync(int id, string userId);
  Task<JournalDto> CreateAsync(
    string title, 
    string content, 
    List<int> tagIds, 
    List<int> categoryIds, 
    string userId);
  Task<JournalDto?> UpdateAsync(int id, string title, string content, int? categoryId, List<string> tags, string userId);
  Task<int> GetCountAsync(string userId, string? search);
  Task<bool> DeleteAsync(int id, string userId);
  Task<bool>RestoreAsync(int id, string userId);
  Task<List<JournalDto>> GetDeletedAsync(string userId);

}