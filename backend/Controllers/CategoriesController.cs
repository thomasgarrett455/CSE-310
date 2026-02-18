using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using JournalApi.Models.DTOs;

[Authorize]
[ApiController]
[Route("api/categories")]
public class CategoriesController : ControllerBase
{
  private readonly ICategoryService _service;

  public CategoriesController(ICategoryService service)
  {
    _service = service;
  }

  private string GetUserId() =>
    User.FindFirstValue(ClaimTypes.NameIdentifier)!;
  
  [HttpGet]
  public async Task<IActionResult> GetAll()
  {
    return Ok(await _service.GetAllAsync(GetUserId()));
  }

  [HttpPost]
  public async Task<IActionResult> Create(CreateCategoryDto dto)
  {
    var category = await _service.CreateAsync(dto, GetUserId());
    return Ok(category);
  }

  [HttpPut("{id}")]
  public async Task<IActionResult> Update(int id, UpdateCategoryDto dto)
  {
    var userId = GetUserId();
    var updated = await _service.UpdateAsync(id, dto, userId);

    if (updated == null)
      return BadRequest(new { message = "Category name alredy exists or category not found" });
    
    return Ok(updated);
  }

  [HttpDelete("{id}")]
  public async Task<IActionResult> Delete(int id)
  {
    var success = await _service.DeleteAsync(id, GetUserId());
    if (!success) return NotFound();
    return NoContent();
  }
}