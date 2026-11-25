namespace LegacyArchive.Application.DTOs.Responses;

public class MediaFileResponse
{
    public Guid Id { get; set; }

    public string OriginalFileName { get; set; } = string.Empty;

    public int Width { get; set; }

    public int Height { get; set; }

    public string ContentType { get; set; } = string.Empty;

    public long SizeBytes { get; set; }

    public int SortOrder { get; set; }

    public DateTime CreatedAt { get; set; }
}
