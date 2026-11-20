using FluentValidation;
using LegacyArchive.Application.DTOs;
using LegacyArchive.Application.DTOs.Responses;
using LegacyArchive.Application.Exceptions;
using LegacyArchive.Application.Interfaces;
using LegacyArchive.Application.Validators.Rules;
using LegacyArchive.Domain.Entities;
using LegacyArchive.Application.Mappings;

namespace LegacyArchive.Application.Services;

public class MediaService : IMediaService
{
    private readonly IValidator<UploadMediaRequest> _validator;
    private readonly IArtworkRepository _artworkRepository;
    private readonly IFileStorage _fileStorage;
    private readonly IMediaFileRepository _mediaRepository;

    public MediaService(IValidator<UploadMediaRequest> validator,
        IArtworkRepository artworkRepository,
        IFileStorage fileStorage,
        IMediaFileRepository mediaRepository)
    {
        _validator = validator;
        _artworkRepository = artworkRepository;
        _fileStorage = fileStorage;
        _mediaRepository = mediaRepository;
    }

    public async Task<MediaFileResponse> UploadAsync(
    Guid artworkId, UploadMediaRequest request, CancellationToken cancellationToken = default)
    {
        await _validator.ValidateAndThrowAsync(request, cancellationToken);

        if (!await _artworkRepository.ExistsAsync(artworkId))
            throw new ArtworkNotFoundException(artworkId);

        var extension = Path.GetExtension(request.FileName).ToLowerInvariant();
        var storagePath = $"artworks/{artworkId}/{Guid.NewGuid()}{extension}";

        await _fileStorage.SaveAsync(request.Content, storagePath, cancellationToken);

        var media = new MediaFile
        {
            ArtworkId = artworkId,
            OriginalFileName = Path.GetFileName(request.FileName),
            StoragePath = storagePath,
            ContentType = MediaRules.AllowedExtensions[extension],
            FileSize = request.Length,
            SortOrder = await _mediaRepository.GetNextSortOrderAsync(artworkId)
        };

        try
        {
            await _mediaRepository.AddAsync(media);
        }
        catch
        {
            await _fileStorage.DeleteAsync(storagePath);
            throw;
        }

        return media.ToResponse();
    }
}
