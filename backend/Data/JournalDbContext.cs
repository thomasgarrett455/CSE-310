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

    public DbSet<JournalTag> JournalTags => Set<JournalTag>();

    public DbSet<JournalCategory> JournalCategories => Set<JournalCategory>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Many-to-many between JournalEntry and Tag
        builder.Entity<JournalEntry>()
            .HasMany(j => j.Tags)
            .WithMany(t => t.Journals);

        // One-to-many between Category and JournalEntry
        builder.Entity<Category>()
            .HasMany(c => c.Journals)
            .WithOne(j => j.Category)
            .HasForeignKey(j => j.CategoryId)
            .OnDelete(DeleteBehavior.SetNull); // optional: keeps journals if category deleted

        builder.Entity<JournalTag>()
            .HasKey(jt => new { jt.JournalEntryId, jt.TagId });

    
        builder.Entity<JournalCategory>()
            .HasKey (jc => new { jc.JournalEntryId, jc.CategoryId });
    }
}
