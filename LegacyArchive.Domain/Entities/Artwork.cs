namespace LegacyArchive.Domain.Entities;

public class Artwork
{
    public Guid Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    public int? Year { get; set; }

    public DateTime CreatedAt { get; set; }

    public Guid CategoryId { get; set; }

    public Category Category { get; set; } = null!;

    public ICollection<MediaFile> MediaFiles { get; set; } = new List<MediaFile>();
}
