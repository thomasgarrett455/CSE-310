using System.ComponentModel.DataAnnotations;

namespace JournalApi.Models.DTOs;

public class UpdateJournalDto
{
  [Required(ErrorMessage = "Title is required")]
  [MaxLength(100, ErrorMessage = "Title cannot exceed 100 characters")]
  public string Title { get; set; } = string.Empty;

  [Required(ErrorMessage = "Content is required")]
  [MaxLength(2000, ErrorMessage = "Content cannot exceed 2000 characters")]
  public string Content { get; set; } = string.Empty;

  public List<int> TagIds { get; set; } = new();
  public List<int> CategoryIds { get; set; } = new();
}