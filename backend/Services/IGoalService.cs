using JournalApi.Models;
using JournalApi.Models.DTOs;

namespace JournalApi.Services;

public interface IGoalService
{
  Task<List<GoalDto>> GetAllAsync(string userId);
  Task<GoalDto?> GetByIdAsync(int id, string userId);
  Task<GoalDto> CreateAsync(CreateGoalDto dto, string userId);
  Task<GoalDto?> UpdateAsync(int id, UpdateGoalDto dto, string userId);
  Task<bool> DeleteAsync(int id, string userId);

  Task<bool> ToggleSaveAsync(int goalId, string userId);
  Task<List<Goal>> GetSavedAsync(string userId);
}