namespace LegacyArchive.Domain.Entities;

public class MediaFile
{
    public Guid Id { get; set; }

    public string OriginalFileName { get; set; } = string.Empty;

    public string StoragePath { get; set; } = string.Empty;

    public string ContentType { get; set; } = string.Empty;

    public long FileSize { get; set; }

    public int SortOrder { get; set; }

    public DateTime CreatedAt { get; set; }

    public Guid ArtworkId { get; set; }

    public Artwork Artwork { get; set; } = null!;
}
