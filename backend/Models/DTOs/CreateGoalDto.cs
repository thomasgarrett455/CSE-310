using System.ComponentModel.DataAnnotations;

namespace JournalApi.Models.DTOs;

public class CreateGoalDto
{
  [Required]
  [MaxLength(200)]
  public string Title { get; set; } = string.Empty;

  [MaxLength(1000)]
  public string? Description { get; set; }

  public DateTime? DueDate { get; set; }

}