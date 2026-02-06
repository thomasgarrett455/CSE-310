using JournalApi.Models.DTOs;

public interface ITagService
{
  Task<List<TagDto>> GetAllAsync(string userId);
  Task<TagDto> CreateAsync(string name, string userId);
  Task<bool> DeleteAsync(int id, string userId);
}