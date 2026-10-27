namespace Application.DTOs;

public class CreateArtworkRequest
{
    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    public int? Year { get; set; }

    public Guid CategoryId { get; set; }
}
