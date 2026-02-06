using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JournalApi.Models;

public class JournalEntry
{
  public int Id { get; set; } // Primary key
  
  [Required(ErrorMessage = "Title is required")]
  [MinLength(3, ErrorMessage = "Title must be at least 3 characters")]  
  public string Title { get; set; } = string.Empty;

  [Required(ErrorMessage = "Title is required")]
  [MinLength(5, ErrorMessage = "Content must be at least 5 characters")]
  public string Content { get; set; } = string.Empty;
  public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

  // Link to the user who created this journal
  [Required]
  public string UserId { get; set; } = string.Empty;

  [ForeignKey("UserId")]
  public ApplicationUser? User { get; set; }
  public bool IsDeleted { get; set; } = false;
  public DateTime? DeletedAt { get; set; }

  public int? CategoryId { get; set; }
  public Category? Category { get; set; }

  public List<JournalTag> JournalTags { get; set; } = new();
  public List<JournalCategory> JournalCategories { get; set; } = new();

  public List<Tag> Tags { get; set; } = new();
}