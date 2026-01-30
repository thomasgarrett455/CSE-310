using JournalApi.Models;
using JournalApi.Models.DTOs;

public interface IJournalService
{
  Task<List<JournalDto>> GetAllAsync(string userId, int page = 1, int pageSize = 10);
  Task<JournalDto?> GetByIdAsync(int id, string userId);
  Task<JournalDto> CreateAsync(string title, string content, string userId);
  Task<JournalDto?> UpdateAsync(int id, string title, string content, string userId);
  Task<bool> DeleteAsync(int id, string userId);

}