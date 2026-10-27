namespace Application.DTOs.Responses;

public class ArtworkResponse
{
    public Guid Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    public int? Year { get; set; }

    public Guid CategoryId { get; set; }

    public DateTime CreatedAt { get; set; }
}
