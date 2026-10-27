using Application.DTOs;
using Application.DTOs.Responses;

namespace Application.Interfaces;

public interface IArtworkService
{
    Task<ArtworkResponse> CreateAsync(
        CreateArtworkRequest request);

    Task<ArtworkResponse?> GetByIdAsync(Guid id);

    Task<List<ArtworkResponse>> GetAllAsync();
}
