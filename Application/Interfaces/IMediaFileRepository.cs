using LegacyArchive.Domain.Entities;

namespace LegacyArchive.Application.Interfaces;

public interface IMediaFileRepository
{
    Task<MediaFile> AddAsync(MediaFile mediaFile);

    Task<int> GetNextSortOrderAsync(Guid artworkId);

    Task<MediaFile?> GetByIdAsync(Guid mediaId);

    Task<List<MediaFile>> GetByArtworkIdAsync(Guid artworkId);

    Task DeleteAsync(MediaFile media);

    Task<List<MediaFile>> GetByArtworkIdForUpdateAsync(Guid artworkId);

    Task UpdateRangeAsync(IEnumerable<MediaFile> mediaFiles);
}
