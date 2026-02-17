using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using JournalApi.Models.DTOs;

[Authorize]
[ApiController]
[Route("api/tags")]
public class TagsController : ControllerBase
{
  private ITagService _service;

  public TagsController(ITagService service)
  {
    _service = service;
  }

  private string GetUserId() =>
    User.FindFirstValue(ClaimTypes.NameIdentifier)!;

    [HttpGet]
    public async Task<IActionResult> GetAll()
  {
    List<TagDto> tags = await _service.GetAllAsync(GetUserId());
    return Ok(tags);
  }

  [HttpPost]
  public async Task<IActionResult> Create(CreateTagDto dto)
  {
    var tag = await _service.CreateAsync(dto.Name, GetUserId());
    return Ok(tag);
  }

  [HttpPut]
  public async Task<IActionResult> Update(int id, UpdateTagDto dto)
  {
    var userId = GetUserId();
    var updated = await _service.UpdateAsync(id, dto, userId);

    if (updated == null)
      return BadRequest(new { message = "Tag name already exists or tag not found" });
    
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