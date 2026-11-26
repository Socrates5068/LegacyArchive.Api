using LegacyArchive.Application.Interfaces;
using LegacyArchive.Domain.Entities;
using LegacyArchive.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LegacyArchive.Infrastructure.Repositories;

public class MediaFileRepository : IMediaFileRepository
{
    private readonly AppDbContext _context;

    public MediaFileRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<MediaFile> AddAsync(MediaFile mediaFile)
    {
        _context.MediaFiles.Add(mediaFile);

        await _context.SaveChangesAsync();

        return mediaFile;
    }

    public async Task<List<MediaFile>> GetByArtworkIdAsync(Guid artworkId)
    {
        var query = _context.MediaFiles.AsNoTracking();

        return await query
            .Where(m => m.ArtworkId == artworkId)
            .OrderBy(m => m.SortOrder)
            .ToListAsync();
    }

    public async Task<MediaFile?> GetByIdAsync(Guid mediaId)
    {
        return await _context.MediaFiles
            .FirstOrDefaultAsync(m => m.Id == mediaId);
    }

    public async Task<int> GetNextSortOrderAsync(Guid artworkId)
    {
        var maxSortOrder = await _context.MediaFiles
            .Where(m => m.ArtworkId == artworkId)
            .MaxAsync(m => (int?)m.SortOrder);

        return (maxSortOrder ?? 0) + 1;
    }

    public async Task DeleteAsync(MediaFile media)
    {
        _context.MediaFiles.Remove(media);
        await _context.SaveChangesAsync();
    }

    public async Task<List<MediaFile>> GetByArtworkIdForUpdateAsync(Guid artworkId)
    {
        return await _context.MediaFiles
            .Where(m => m.ArtworkId == artworkId)
            .ToListAsync();
    }

    public async Task UpdateRangeAsync(IEnumerable<MediaFile> mediaFiles)
    {
        await _context.SaveChangesAsync();
    }
}
