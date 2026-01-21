using Microsoft.EntityFrameworkCore;
using JournalApi.Models;

namespace JournalApi.Data;

public class JournalDbContext : DbContext
{
    public JournalDbContext(DbContextOptions<JournalDbContext> options)
        : base(options)
  {
    
  }

  public DbSet<JournalEntry> JournalEntries => Set<JournalEntry>();
}