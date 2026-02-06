using JournalApi.Data;
using JournalApi.Models;
using JournalApi.Models.DTOs;
using Microsoft.EntityFrameworkCore;

public class CategoryService : ICategoryService
{
  private readonly JournalDbContext _db;

  public CategoryService(JournalDbContext db)
  {
    _db = db;
  }

  public async Task<List<CategoryDto>> GetAllAsync(string userId)
  {
    return await _db.Categories
      .Where(c => c.UserId == userId)
      .OrderBy(c => c.Name)
      .Select(c => new CategoryDto
      {
        Id = c.Id,
        Name = c.Name
      })
      .ToListAsync();
  }

  public async Task<CategoryDto> CreateAsync(string name, string userId)
  {
    var category = new Category
    {
      Name = name,
      UserId = userId
    };

    _db.Categories.Add(category);
    await _db.SaveChangesAsync();

    return new CategoryDto
    {
      Id = category.Id,
      Name = category.Name
    };
  }

  public async Task<bool> DeleteAsync(int id, string userId)
  {
    var category = await _db.Categories
      .FirstOrDefaultAsync(c =>
        c.Id == id &&
        c.UserId == userId);

    if (category == null)
      return false;

    _db.Categories.Remove(category);
    await _db.SaveChangesAsync();
    return true;
  }
}