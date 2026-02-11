using System.ComponentModel.DataAnnotations;

namespace JournalApi.Models;

public class Tag
{
  public int Id { get; set; }

  [Required]
  public string Name { get; set; } = string.Empty;
  public string UserId { get; set; } = string.Empty;

  public List<JournalTag> JournalTags { get; set; } = new();

}