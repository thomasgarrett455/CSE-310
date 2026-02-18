using JournalApi.Models.DTOs;
using JournalApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace JournalApi.Controllers;

[Authorize]
[ApiController]
[Route("api/goals")]
public class GoalsController : ControllerBase
{
  private readonly IGoalService _service;

  public GoalsController(IGoalService service)
  {
    _service = service;
  }

  private string GetUserId() =>
    User.FindFirstValue(ClaimTypes.NameIdentifier)!;

  [HttpGet]
  public async Task<IActionResult> GetAll()
  {
    var goals = await _service.GetAllAsync(GetUserId());
    return Ok(goals);
  }

  [HttpPost]
  public async Task<IActionResult> Create(CreateGoalDto dto)
  {
    var goal = await _service.CreateAsync(dto, GetUserId());
    return CreatedAtAction(nameof(GetAll), new { id = goal.Id }, goal);
  }

  [HttpPut("{id}")]
  public async Task<IActionResult> Update(int id, UpdateGoalDto dto)
  {
    var goal = await _service.UpdateAsync(id, dto, GetUserId());
    if (goal == null) return NotFound();
    return Ok(goal);
  }

  [HttpDelete("{id}")]
  public async Task<IActionResult> Delete(int id)
  {
    var deleted = await _service.DeleteAsync(id, GetUserId());
    if (!deleted) return NotFound();
    return NoContent();
  }
}
