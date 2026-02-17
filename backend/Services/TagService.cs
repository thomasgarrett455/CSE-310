using JournalApi.Data;
using JournalApi.Models;
using JournalApi.Models.DTOs;
using Microsoft.EntityFrameworkCore;

public class TagService : ITagService
{
  private readonly JournalDbContext _db;

  public TagService(JournalDbContext db)
  {
    _db = db;
  }

  public async Task<List<TagDto>> GetAllAsync(string userId)
  {
    return await _db.Tags
      .Where(t => t.UserId == userId)
      .OrderBy(t => t.Name)
      .Select(t => new TagDto
      {
          Id = t.Id,
          Name = t.Name
      })
      .ToListAsync();
  }

  public async Task<TagDto> CreateAsync(string name, string userId)
  {
    var tag = new Tag
    {
      Name = name,
      UserId = userId
    };

    _db.Tags.Add(tag);
    await _db.SaveChangesAsync();

    return new TagDto
    {
      Id = tag.Id,
      Name = tag.Name
    };
  }

  public async Task<TagDto?> UpdateAsync(int id, UpdateTagDto dto, string userId)
  {
  
    var tag = await _db.Tags
      .Include(t => t.JournalTags)
      .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

    if (tag == null) return null;

    var existingTag = await _db.Tags
      .Include(t => t.JournalTags)
      .FirstOrDefaultAsync(t => t.UserId == userId && t.Name == dto.Name && t.Id != id);

    if (existingTag != null)
    {
      var journalTags = await _db.JournalTags
        .Where(jt => jt.TagId == id)
        .ToListAsync();

      foreach (var jt in journalTags)
      {
        if (!await _db.JournalTags.AnyAsync(x => x.JournalEntryId == jt.JournalEntryId && x.TagId == existingTag.Id))
        {
          jt.TagId = existingTag.Id;
        }
        else
        {
          _db.JournalTags.Remove(jt);
        }
      }

      _db.Tags.Remove(tag);
      await _db.SaveChangesAsync();
      
      return new TagDto { Id = existingTag.Id, Name = existingTag.Name };
    }

    tag.Name = dto.Name;
    await _db.SaveChangesAsync();

    return new TagDto { Id = tag.Id, Name = tag.Name };
  }

  public async Task<bool> DeleteAsync(int id, string userId)
  {
    var tag = await _db.Tags
      .FirstOrDefaultAsync(t =>
          t.Id == id &&
          t.UserId == userId);

    if (tag == null)
      return false;
    
    _db.Tags.Remove(tag);
    await _db.SaveChangesAsync();
    return true;
  }
}