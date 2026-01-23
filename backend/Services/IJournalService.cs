using JournalApi.Models;

public interface IJournalService
{
  Task<List<JournalEntry>> GetAllAsync();
  Task<JournalEntry?> GetByIdAsync(int id);
  Task<JournalEntry> CreateAsync(string title, string content);
  Task<JournalEntry?> UpdateAsync(int id, string title, string content);
  Task<bool> DeleteAsync(int id);
}