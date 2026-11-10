using LegacyArchive.Application.DTOs.Responses;
using LegacyArchive.Domain.Entities;

namespace LegacyArchive.Application.Mappings;

public static class ArtworkMappings
{
    public static ArtworkResponse ToResponse(this Artwork artwork) => new()
    {
        Id = artwork.Id,
        Title = artwork.Title,
        Description = artwork.Description,
        Year = artwork.Year,
        CreatedAt = artwork.CreatedAt,
        Category = artwork.Category.ToDto()
    };
}
