namespace JournalApi.Models.DTOs;

public class UpdateJournalWithMetaDto
{
  public string Title { get; set; } = string.Empty;
  public string Content { get; set; } = string.Empty;

  public int? CategoryId { get; set; }

  public List<string> Tags { get; set; } = new();
}