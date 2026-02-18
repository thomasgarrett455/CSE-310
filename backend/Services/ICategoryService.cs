using JournalApi.Models.DTOs;

public interface ICategoryService
{
  Task<List<CategoryDto>> GetAllAsync(string userId);
  Task<CategoryDto> CreateAsync(CreateCategoryDto dto, string userId);
  Task<bool> DeleteAsync(int id, string userId);

  Task<CategoryDto?> UpdateAsync(int id, UpdateCategoryDto dto, string userId);
}