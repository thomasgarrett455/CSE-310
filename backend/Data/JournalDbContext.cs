using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using JournalApi.Models;

namespace JournalApi.Data;

public class JournalDbContext : IdentityDbContext<ApplicationUser>
{
    public JournalDbContext(DbContextOptions<JournalDbContext> options)
        : base(options)
  {
    
  }

  public DbSet<JournalEntry> JournalEntries => Set<JournalEntry>();

  public DbSet<Category> Categories => Set<Category>();
  public DbSet<Tag> Tags => Set<Tag>();

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    base.OnModelCreating(modelBuilder);

    modelBuilder.Entity<JournalEntry>()
        .HasMany(j => j.Tags)
        .WithMany(t => t.Journals);
  }
}