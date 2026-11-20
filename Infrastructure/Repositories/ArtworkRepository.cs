using LegacyArchive.Application.Interfaces;
using LegacyArchive.Domain.Entities;
using LegacyArchive.Infrastructure.Extensions;
using LegacyArchive.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Xml.Linq;


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
            .Include(a => a.Category)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<(List<Artwork> Items, int TotalCount)> GetPagedAsync(int page, int pageSize)
    {
        var query = _context.Artworks.AsNoTracking();

        var totalCount = await query.CountAsync();

        var items = await query
            .Include(a => a.Category)
            .OrderByDescending(a => a.CreatedAt)
            .ThenBy(a => a.Id)
            .Paginate(page, pageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    public async Task UpdateAsync(Artwork artwork)
    {
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Artwork artwork)
    {
        _context.Artworks.Remove(artwork);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _context.Artworks
           .AnyAsync(a => a.Id == id);
    }
}
