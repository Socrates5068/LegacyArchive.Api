using LegacyArchive.Application.DTOs;
using LegacyArchive.Application.DTOs.Responses;

namespace LegacyArchive.Application.Interfaces;

public interface IArtworkService
{
    Task<ArtworkResponse> CreateAsync(
        CreateArtworkRequest request);

    Task<ArtworkResponse?> GetByIdAsync(Guid id);

    Task<PagedResponse<ArtworkResponse>> GetAllAsync(PaginationQuery query);

    Task<ArtworkResponse> UpdateAsync(Guid id, UpdateArtworkRequest request);

    Task DeleteAsync(Guid id);
}
