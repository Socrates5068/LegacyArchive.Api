using Application.DTOs;
using Application.DTOs.Responses;
using Application.Interfaces;
using LegacyArchive.Domain.Entities;

namespace Application.Services;

public class ArtworkService : IArtworkService
{
    private readonly IArtworkRepository _repository;

    public ArtworkService(IArtworkRepository repository)
    {
        _repository = repository;
    }

    public async Task<ArtworkResponse> CreateAsync(
    CreateArtworkRequest request)
    {
        var artwork = new Artwork
        {
            Title = request.Title,
            Description = request.Description,
            Year = request.Year,
            CategoryId = request.CategoryId,
        };

        var createdArtwork =
            await _repository.AddAsync(artwork);

        return new ArtworkResponse
        {
            Id = createdArtwork.Id,
            Title = createdArtwork.Title,
            Description = createdArtwork.Description,
            Year = createdArtwork.Year,
            CreatedAt = createdArtwork.CreatedAt,
            CategoryId = createdArtwork.CategoryId
        };
    }


    public async Task<ArtworkResponse?> GetByIdAsync(Guid id)
    {
        var artwork =
            await _repository.GetByIdAsync(id);

        if (artwork is null)
            return null;

        return new ArtworkResponse
        {
            Id = artwork.Id,
            Title = artwork.Title,
            Description = artwork.Description,
            CreatedAt = artwork.CreatedAt
        };
    }

    public async Task<List<ArtworkResponse>> GetAllAsync()
    {
        var artworks = await _repository.GetAllAsync();

        return artworks.Select(a => new ArtworkResponse
        {
            Id = a.Id,
            Title = a.Title,
            Description = a.Description,
            Year = a.Year,
            CreatedAt = a.CreatedAt,
            CategoryId = a.CategoryId
        }).ToList();
    }
}
