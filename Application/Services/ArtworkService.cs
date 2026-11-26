using FluentValidation;
using LegacyArchive.Application.DTOs;
using LegacyArchive.Application.DTOs.Responses;
using LegacyArchive.Application.Exceptions;
using LegacyArchive.Application.Interfaces;
using LegacyArchive.Application.Mappings;
using LegacyArchive.Application.Storage;
using LegacyArchive.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace LegacyArchive.Application.Services;

public class ArtworkService : IArtworkService
{
    private readonly IArtworkRepository _repository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IValidator<CreateArtworkRequest> _createValidator;
    private readonly IValidator<UpdateArtworkRequest> _updateValidator;
    private readonly IValidator<PaginationQuery> _paginationValidator;
    private readonly IFileStorage _fileStorage;
    private readonly ILogger<ArtworkService> _logger;

    public ArtworkService(
        IArtworkRepository repository,
        ICategoryRepository categoryRepository,
        IValidator<CreateArtworkRequest> createValidator,
        IValidator<UpdateArtworkRequest> updateValidator,
        IValidator<PaginationQuery> paginationValidator,
        IFileStorage fileStorage,
        ILogger<ArtworkService> logger)
    {
        _repository = repository;
        _categoryRepository = categoryRepository;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
        _paginationValidator = paginationValidator;
        _fileStorage = fileStorage;
        _logger = logger;   
    }

    public async Task<ArtworkResponse> CreateAsync(
        CreateArtworkRequest request)
    {
        await _createValidator.ValidateAndThrowAsync(request);

        var category = await _categoryRepository.GetByIdAsync(request.CategoryId)
            ?? throw new CategoryNotFoundException(request.CategoryId);

        


        var artwork = new Artwork
        {
            Title = request.Title.Trim(),
            Description = string.IsNullOrWhiteSpace(request.Description)
                ? null
                : request.Description.Trim(),
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

    public async Task<PagedResponse<ArtworkResponse>> GetAllAsync(PaginationQuery query)
    {
        await _paginationValidator.ValidateAndThrowAsync(query);

        var (items, totalCount) = await _repository.GetPagedAsync(query.Page, query.PageSize);

        return new PagedResponse<ArtworkResponse>
        {
            Items = items.Select(a => a.ToResponse()).ToList(),
            Page = query.Page,
            PageSize = query.PageSize,
            TotalCount = totalCount
        };
    }

    public async Task<ArtworkResponse> UpdateAsync(Guid id, UpdateArtworkRequest request)
    {
        await _updateValidator.ValidateAndThrowAsync(request);

        var artwork = await _repository.GetByIdAsync(id)
            ?? throw new ArtworkNotFoundException(id);

        if (artwork.CategoryId != request.CategoryId)
        {
            var category = await _categoryRepository.GetByIdAsync(request.CategoryId)
                ?? throw new CategoryNotFoundException(request.CategoryId);

            artwork.Category = category;
        }

        artwork.Title = request.Title.Trim();
        artwork.Description = string.IsNullOrWhiteSpace(request.Description)
            ? null
            : request.Description.Trim();
        artwork.Year = request.Year;

        await _repository.UpdateAsync(artwork);

        return artwork.ToResponse();
    }

    public async Task DeleteAsync(Guid id)
    {
        var artwork = await _repository.GetByIdAsync(id)
            ?? throw new ArtworkNotFoundException(id);

        // 1. Base de datos: borra la obra, y el Cascade borra sus MediaFiles
        await _repository.DeleteAsync(artwork);

        // 2. Disco: una sola carpeta con todos los escaneos de la obra
        var folder = MediaPaths.GetArtworkFolder(id);

        try
        {
            await _fileStorage.DeleteFolderAsync(folder);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex,
                "Artwork {ArtworkId} was deleted from the database but its folder {Folder} could not be removed.",
                id, folder);
        }
    }
}
