using LegacyArchive.Application.DTOs.Responses;
using LegacyArchive.Domain.Entities;

namespace LegacyArchive.Application.Mappings;

public static class MediaFileMappings
{
    public static MediaFileResponse ToResponse(this MediaFile mediaFile) => new()
    {
        Id = mediaFile.Id,
        OriginalFileName = mediaFile.OriginalFileName,
        ContentType = mediaFile.ContentType,
        SortOrder = mediaFile.SortOrder,
        SizeBytes = mediaFile.FileSize,
        CreatedAt = mediaFile.CreatedAt,
    };
}
