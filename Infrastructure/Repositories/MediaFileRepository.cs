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

    public async Task<int> GetNextSortOrderAsync(Guid artworkId)
    {
        var maxSortOrder = await _context.MediaFiles
            .Where(m => m.ArtworkId == artworkId)
            .MaxAsync(m => (int?)m.SortOrder);

        return (maxSortOrder ?? 0) + 1;
    }
}
