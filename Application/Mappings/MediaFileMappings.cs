using LegacyArchive.Application.DTOs.Responses;
using LegacyArchive.Domain.Entities;

namespace LegacyArchive.Application.Mappings;

public static class MediaFileMappings
{
    public static MediaFileResponse ToResponse(this MediaFile mediaFile)
    {
        return new MediaFileResponse
        {
            Id = mediaFile.Id,
            OriginalFileName = mediaFile.OriginalFileName,
            Width = mediaFile.Width,
            Height = mediaFile.Height,
            ContentType = mediaFile.ContentType,
            SortOrder = mediaFile.SortOrder,
            SizeBytes = mediaFile.FileSize,
            ThumbnailUrl = mediaFile.GetThumbnailUrl(),
            PreviewUrl = $"{GetBaseUrl(mediaFile)}/preview",
            OriginalUrl = $"{GetBaseUrl(mediaFile)}/original",
            CreatedAt = mediaFile.CreatedAt
        };
    }

    public static string GetThumbnailUrl(this MediaFile mediaFile)
    => $"{GetBaseUrl(mediaFile)}/thumbnail";

    private static string GetBaseUrl(MediaFile mediaFile)
        => $"/api/artworks/{mediaFile.ArtworkId}/media/{mediaFile.Id}";
}
