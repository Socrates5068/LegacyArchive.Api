using LegacyArchive.Application.Mappings;
using LegacyArchive.Application.DTOs;
using LegacyArchive.Application.DTOs.Responses;
using LegacyArchive.Application.Exceptions;
using LegacyArchive.Application.Interfaces;
using LegacyArchive.Domain.Entities;

namespace LegacyArchive.Application.Services;

public class ArtworkService : IArtworkService
{
    private readonly IArtworkRepository _repository;
    private readonly ICategoryRepository _categoryRepository;

    public ArtworkService(
        IArtworkRepository repository,
        ICategoryRepository categoryRepository)
    {
        _repository = repository;
        _categoryRepository = categoryRepository;
    }

    public async Task<ArtworkResponse> CreateAsync(
        CreateArtworkRequest request)
    {
        var category = await _categoryRepository.GetByIdAsync(request.CategoryId)
            ?? throw new CategoryNotFoundException(request.CategoryId);

        var artwork = new Artwork
        {
            Title = request.Title,
            Description = request.Description,
            Year = request.Year,
            Category = category
        };

        var created = await _repository.AddAsync(artwork);

        return created.ToResponse();
    }


    public async Task<ArtworkResponse?> GetByIdAsync(Guid id)
    {
        var artwork = await _repository.GetByIdAsync(id);

        return artwork?.ToResponse();
    }

    public async Task<List<ArtworkResponse>> GetAllAsync()
    {
        var artworks = await _repository.GetAllAsync();

        return artworks.Select(a => a.ToResponse()).ToList();
    }

    public async Task<ArtworkResponse> UpdateAsync(Guid id, UpdateArtworkRequest request)
    {
        var artwork = await _repository.GetByIdAsync(id)
            ?? throw new ArtworkNotFoundException(id);

        if (artwork.CategoryId != request.CategoryId)
        {
            var category = await _categoryRepository.GetByIdAsync(request.CategoryId)
                ?? throw new CategoryNotFoundException(request.CategoryId);

            artwork.Category = category;
        }

        artwork.Title = request.Title;
        artwork.Description = request.Description;
        artwork.Year = request.Year;

        await _repository.UpdateAsync(artwork);

        return artwork.ToResponse();
    }

    public async Task DeleteAsync(Guid id)
    {
        var artwork = await _repository.GetByIdAsync(id)
            ?? throw new ArtworkNotFoundException(id);

        await _repository.DeleteAsync(artwork);
    }
}
