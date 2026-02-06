using JournalApi.Models.DTOs;

public interface ICategoryService
{
  Task<List<CategoryDto>> GetAllAsync(string userId);
  Task<CategoryDto> CreateAsync(string name, string userId);
  Task<bool> DeleteAsync(int id, string userId);
}