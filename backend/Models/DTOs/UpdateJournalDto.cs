using System.ComponentModel.DataAnnotations;

namespace JournalApi.Models.DTOs;

public class UpdateJournalDto
{
  [Required]
  [MaxLength(100)]
  public string Title { get; set; } = string.Empty;

  [Required]
  [MaxLength(2000)]
  public string Content { get; set; } = string.Empty;

  public List<int> TagIds { get; set; } = new();
  public List<int> CategoryIds { get; set; } = new();
}