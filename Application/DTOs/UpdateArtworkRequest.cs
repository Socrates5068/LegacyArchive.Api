using System.ComponentModel.DataAnnotations;

namespace LegacyArchive.Application.DTOs;

public class UpdateArtworkRequest
{
    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(2000)]
    public string? Description { get; set; }

    [Range(0, 3000)]
    public int? Year { get; set; }

    [Required]
    public Guid CategoryId { get; set; }
}
