using System.ComponentModel.DataAnnotations;

namespace JournalApi.Models;

public class Category
{
    public int Id { get; set; }

    [Required]
    public string Name { get; set; } = string.Empty;

    public string UserId { get; set; } = string.Empty;

    public List<JournalCategory> JournalCategories { get; set; } = new();

}
