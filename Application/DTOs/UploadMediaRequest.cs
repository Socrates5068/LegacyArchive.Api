namespace LegacyArchive.Application.DTOs;

public class UploadMediaRequest
{
    public required Stream Content { get; init; }

    public required string FileName { get; init; }

    public long Length { get; init; }
}
