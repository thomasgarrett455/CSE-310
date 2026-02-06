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
    var category = await _service.CreateAsync(dto.Name, GetUserId());
    return Ok(category);
  }

  [HttpDelete("{id}")]
  public async Task<IActionResult> Delete(int id)
  {
    var success = await _service.DeleteAsync(id, GetUserId());
    if (!success) return NotFound();
    return NoContent();
  }
}