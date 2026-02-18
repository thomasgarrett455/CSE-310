using System.ComponentModel.DataAnnotations;

namespace JournalApi.Models.DTOs;

public class CreateTagDto
{
  [Required(ErrorMessage = "Name is required")]
  [MaxLength(50, ErrorMessage = "Tag name cannot exceed 50 characters")]
  public string Name { get; set; } = string.Empty;
}