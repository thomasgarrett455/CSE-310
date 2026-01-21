using System.ComponentModel.DataAnnotations;

namespace JournalApi.Models;

public class JournalEntry
{
  public int Id { get; set; }
  
  [Required(ErrorMessage = "Title is required")]
  [MinLength(3, ErrorMessage = "Title must be at least 3 characters")]  
  public string Title { get; set; } = string.Empty;

  [Required(ErrorMessage = "Title is required")]
  [MinLength(5, ErrorMessage = "Content must be at least 5 characters")]
  public string Content { get; set; } = string.Empty;
  public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}