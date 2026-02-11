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

        builder.Entity<JournalTag>()
            .HasKey(jt => new { jt.JournalEntryId, jt.TagId });

        
        builder.Entity<JournalTag>()
            .HasOne(jt => jt.JournalEntry)
            .WithMany(j => j.JournalTags)
            .HasForeignKey(jt => jt.JournalEntryId);
        
        builder.Entity<JournalTag>()
            .HasOne(jt => jt.Tag)
            .WithMany(t => t.JournalTags)
            .HasForeignKey(jt => jt.TagId);

        builder.Entity<JournalCategory>()
            .HasKey (jc => new { jc.JournalEntryId, jc.CategoryId });

        builder.Entity<JournalCategory>()
            .HasOne(jc => jc.JournalEntry)
            .WithMany(j => j.JournalCategories)
            .HasForeignKey(jc => jc.JournalEntryId);
        
        builder.Entity<JournalCategory>()
            .HasOne(jc => jc.Category)
            .WithMany(c => c.JournalCategories)
            .HasForeignKey(jc => jc.CategoryId);
        // One-to-many between Category and JournalEntry
        builder.Entity<Category>()
            .HasMany(c => c.Journals)
            .WithOne(j => j.Category)
            .HasForeignKey(j => j.CategoryId)
            .OnDelete(DeleteBehavior.SetNull); // optional: keeps journals if category deleted


    
    }
}
