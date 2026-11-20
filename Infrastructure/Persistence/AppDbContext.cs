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

        modelBuilder.Entity<Artwork>()
            .HasOne(a => a.Category)
            .WithMany(c => c.Artworks)
            .HasForeignKey(a => a.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<MediaFile>(entity =>
        {
            entity.Property(m => m.OriginalFileName).HasMaxLength(255);
            entity.Property(m => m.StoragePath).HasMaxLength(500);
            entity.Property(m => m.ContentType).HasMaxLength(100);

            entity.Property(m => m.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .ValueGeneratedOnAdd();

            entity.HasOne(m => m.Artwork)
                .WithMany(a => a.MediaFiles)
                .HasForeignKey(m => m.ArtworkId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        base.OnModelCreating(modelBuilder);
    }

    public DbSet<Artwork> Artworks => Set<Artwork>();

    public DbSet<Category> Categories => Set<Category>();

    public DbSet<MediaFile> MediaFiles => Set<MediaFile>();
}
