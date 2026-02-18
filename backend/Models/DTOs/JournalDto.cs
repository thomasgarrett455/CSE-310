namespace JournalApi.Models.DTOs;

public class JournalDto
{
  public int Id { get; set; }
  public string Title { get; set; } = string.Empty;
  public string Content { get; set; } = string.Empty;
  public DateTime CreatedAt { get; set; }
  
  // New properties
  public List<TagDto> Tags { get; set; } = new();
  public List<CategoryDto> Categories { get; set; } = new();
}