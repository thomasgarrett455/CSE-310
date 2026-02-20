using System.ComponentModel.DataAnnotations;

namespace JournalApi.Models;

public class Goal
{
  public int Id { get; set; }

  [Required]
  [MaxLength(200)]
  public string Title { get; set; } = string.Empty;

  [MaxLength(1000)]
  public string? Description { get; set; }

  public bool IsCompleted { get; set; } = false;

  public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

  public DateTime? DueDate { get; set; }

  public string UserId { get; set; } = string.Empty;

  public bool IsDeleted { get; set; } = false;
    public bool IsSaved { get; set; } = false;

}