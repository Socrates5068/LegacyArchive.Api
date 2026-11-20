using LegacyArchive.Application.DTOs;
using LegacyArchive.Application.DTOs.Responses;

namespace LegacyArchive.Application.Interfaces;

public interface IMediaService
{
    Task<MediaFileResponse> UploadAsync(
        Guid artworkId, UploadMediaRequest file,
        CancellationToken cancellationToken = default);
}
