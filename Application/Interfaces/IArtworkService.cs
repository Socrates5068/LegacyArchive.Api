using LegacyArchive.Application.DTOs;
using LegacyArchive.Application.DTOs.Responses;

namespace LegacyArchive.Application.Interfaces;

public interface IArtworkService
{
    Task<ArtworkResponse> CreateAsync(
        CreateArtworkRequest request);

    Task<ArtworkResponse?> GetByIdAsync(Guid id);

    Task<List<ArtworkResponse>> GetAllAsync();
}
