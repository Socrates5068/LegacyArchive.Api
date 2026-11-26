using LegacyArchive.Application.DTOs;
using LegacyArchive.Application.DTOs.Responses;
using LegacyArchive.Application.Models;

namespace LegacyArchive.Application.Interfaces;

public interface IMediaService
{
    Task<MediaFileResponse> UploadAsync(
        Guid artworkId, UploadMediaRequest file,
        CancellationToken cancellationToken = default);

    Task<List<MediaFileResponse>> GetByArtworkAsync(
        Guid artworkId, CancellationToken cancellationToken = default);

    Task<MediaContent> GetContentAsync(
        Guid artworkId, Guid mediaId, MediaVariant variant,
        CancellationToken cancellationToken = default);
}
