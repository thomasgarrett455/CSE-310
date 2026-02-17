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

  public async Task<CategoryDto> CreateAsync(CreateCategoryDto dto, string userId)
  {
    var category = new Category
    {
      Name = dto.Name,
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

  public async Task<CategoryDto?> UpdateAsync(int id, UpdateCategoryDto dto, string userId)
  {
    var category = await _db.Categories
      .Include(c => c.JournalCategories)
      .FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId);

    if (category == null) return null;

    var existingCategory = await _db.Categories
      .Include(c => c.JournalCategories)
      .FirstOrDefaultAsync(c => c.UserId == userId && c.Name == dto.Name && c.Id != id);
    
    if (existingCategory != null)
    {
      var journalCategories = await _db.JournalCategories
        .Where(jc => jc.CategoryId == id)
        .ToListAsync();

      
      foreach (var jc in journalCategories)
      {
        if (!await _db.JournalCategories.AnyAsync(x => x.JournalEntryId == jc.JournalEntryId && x.CategoryId == existingCategory.Id))
        {
          jc.CategoryId = existingCategory.Id;
        }
        else
        {
          _db.JournalCategories.Remove(jc);
        }
      }
      _db.Categories.Remove(category);
      await _db.SaveChangesAsync();

      return new CategoryDto { Id = existingCategory.Id, Name = existingCategory.Name };
    }
    category.Name = dto.Name;
    await _db.SaveChangesAsync();

    return new CategoryDto { Id = category.Id, Name = category.Name };
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