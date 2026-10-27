using LegacyArchive.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LegacyArchive.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(
        DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Artwork>()
            .Property(a => a.CreatedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP")
            .ValueGeneratedOnAdd();

        base.OnModelCreating(modelBuilder);
    }

    public DbSet<Artwork> Artworks => Set<Artwork>();

    public DbSet<Category> Categories => Set<Category>();
}
