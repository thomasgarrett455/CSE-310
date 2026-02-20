using JournalApi.Data;
using JournalApi.Models;
using JournalApi.Models.DTOs;
using Microsoft.EntityFrameworkCore;

namespace JournalApi.Services;

public class GoalService : IGoalService
{
  private readonly JournalDbContext _db;
  private readonly ILogger<GoalService> _logger;

  public GoalService(JournalDbContext db, ILogger<GoalService> logger)
  {
    _db = db;
    _logger = logger;
  }

  private static GoalDto MapToDto(Goal goal) => new()
  {
    Id = goal.Id,
    Title = goal.Title,
    Description = goal.Description,
    IsCompleted = goal.IsCompleted,
    CreatedAt = goal.CreatedAt,
    DueDate = goal.DueDate
  };

  public async Task<List<GoalDto>> GetAllAsync(string userId)
  {
    try
    {
      return await _db.Goals
          .Where(g => g.UserId == userId && !g.IsDeleted)
          .OrderByDescending(g => g.CreatedAt)
          .Select(g => MapToDto(g))
          .ToListAsync();
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error fetching goals for user {UserId}", userId);
      throw;
    }
  }

  public async Task<GoalDto?> GetByIdAsync(int id, string userId)
  {
    try
    {
      var goal = await _db.Goals
        .FirstOrDefaultAsync(g => g.Id == id && g.UserId == userId && !g.IsDeleted);

      return goal == null ? null : MapToDto(goal);
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error fetching goal {GoalId}", id);
      throw;
    }
  }

  public async Task<GoalDto> CreateAsync(CreateGoalDto dto, string userId)
  {
    try
    {
      var goal = new Goal
      {
        Title = dto.Title,
        Description = dto.Description,
        DueDate = dto.DueDate,
        UserId = userId
      };

      _db.Goals.Add(goal);
      await _db.SaveChangesAsync();

      return MapToDto(goal);
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error creating goal for user {UserId}", userId);
      throw;
    }
  }

  public async Task<GoalDto?> UpdateAsync(int id, UpdateGoalDto dto, string userId)
  {
    try
    {
      var goal = await _db.Goals
        .FirstOrDefaultAsync(g => g.Id == id && g.UserId == userId && !g.IsDeleted);

      if (goal == null) return null;

      goal.Title = dto.Title;
      goal.Description = dto.Description;
      goal.IsCompleted = dto.IsCompleted;
      goal.DueDate = dto.DueDate;

      await _db.SaveChangesAsync();

      return MapToDto(goal);
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error updating goal {GoalId}", id);
      throw;
    }
  }

  public async Task<bool> DeleteAsync(int id, string userId)
  {
    try
    {
      var goal = await _db.Goals
        .FirstOrDefaultAsync(g => g.Id == id && g.UserId == userId);

      if (goal == null) return false;

      goal.IsDeleted = true;

      await _db.SaveChangesAsync();
      return true;
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error deleting goal {GoalId}", id);
      throw;
    }

    }
      public async Task<bool> ToggleSaveAsync(int goalId, string userId)
    {
      var goal = await _db.Goals
        .FirstOrDefaultAsync(g => g.Id == goalId && g.UserId == userId);

      if (goal == null)
        return false;
      
      goal.IsSaved = !goal.IsSaved;
      await _db.SaveChangesAsync();

      return true;
  }

  public async Task<List<Goal>>GetSavedAsync(string userId)
  {
    return await _db.Goals
      .Where(g => g.UserId == userId && g.IsSaved && !g.IsDeleted)
      .ToListAsync();
  }
}