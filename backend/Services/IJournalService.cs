using JournalApi.Models;
using JournalApi.Models.DTOs;

public interface IJournalService
{
  Task<List<JournalDto>> GetAllAsync(
    string userId, 
    string? search, 
    string sort,
    int page = 1, 
    int pageSize = 10);
  Task<JournalDto?> GetByIdAsync(int id, string userId);
  Task<JournalDto> CreateAsync(string title, string content, int? categoryId, List<string> tags, string userId);
  Task<JournalDto?> UpdateAsync(int id, string title, string content, int? categoryId, List<string> tags, string userId);
  Task<int> GetCountAsync(string userId, string? search);
  Task<bool> DeleteAsync(int id, string userId);
  Task<bool>RestoreAsync(int id, string userId);
  Task<List<JournalDto>> GetDeletedAsync(string userId);

}