using Application.Interfaces;
using LegacyArchive.Domain.Entities;
using LegacyArchive.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;


namespace LegacyArchive.Infrastructure.Repositories;

public class ArtworkRepository : IArtworkRepository
{
    private readonly AppDbContext _context;

    public ArtworkRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Artwork> AddAsync(Artwork artwork)
    {
        _context.Artworks.Add(artwork);

        await _context.SaveChangesAsync();

        return artwork;
    }

    public async Task<Artwork?> GetByIdAsync(Guid id)
    {
        return await _context.Artworks
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<List<Artwork>> GetAllAsync()
    {
        return await _context.Artworks.ToListAsync();
    }
}
