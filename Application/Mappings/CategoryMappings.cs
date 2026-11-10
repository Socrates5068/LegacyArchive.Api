using LegacyArchive.Application.DTOs.Responses;
using LegacyArchive.Domain.Entities;

namespace LegacyArchive.Application.Mappings;

public static class CategoryMappings
{
    public static CategoryDto ToDto(this Category category) => new()
    {
        Id = category.Id,
        Name = category.Name
    };

    public static CategoryResponse ToResponse(this Category category) => new()
    {
        Id = category.Id,
        Name = category.Name
    };
}
