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

    public DbSet<Artwork> Artworks => Set<Artwork>();

    public DbSet<Category> Categories => Set<Category>();
}
