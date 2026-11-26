using FluentValidation;
using LegacyArchive.Application.DTOs;
using LegacyArchive.Application.DTOs.Responses;
using LegacyArchive.Application.Exceptions;
using LegacyArchive.Application.Interfaces;
using LegacyArchive.Application.Mappings;
using LegacyArchive.Application.Models;
using LegacyArchive.Application.Validators.Rules;
using LegacyArchive.Application.Storage;
using LegacyArchive.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace LegacyArchive.Application.Services;

public class MediaService : IMediaService
{
    private readonly IValidator<UploadMediaRequest> _uploadValidator;
    private readonly IValidator<ReorderMediaRequest> _reorderValidator;
    private readonly IArtworkRepository _artworkRepository;
    private readonly IFileStorage _fileStorage;
    private readonly IMediaFileRepository _mediaRepository;
    private readonly IImageProcessor _imageProcessor;
    private readonly ILogger<MediaService> _logger;

    public MediaService(
        IValidator<UploadMediaRequest> uploadValidator,
        IValidator<ReorderMediaRequest> reorderValidator,
        IArtworkRepository artworkRepository,
        IFileStorage fileStorage,
        IMediaFileRepository mediaRepository,
        IImageProcessor imageProcessor,
        ILogger<MediaService> logger)
    {
        _uploadValidator = uploadValidator;
        _reorderValidator = reorderValidator;
        _artworkRepository = artworkRepository;
        _fileStorage = fileStorage;
        _mediaRepository = mediaRepository;
        _imageProcessor = imageProcessor;
        _logger = logger;
    }

    public async Task<MediaFileResponse> UploadAsync(
    Guid artworkId, UploadMediaRequest request, CancellationToken cancellationToken = default)
    {
        await _uploadValidator.ValidateAndThrowAsync(request, cancellationToken);

        if (!await _artworkRepository.ExistsAsync(artworkId))
            throw new ArtworkNotFoundException(artworkId);

        var mediaId = Guid.NewGuid();
        var folder = MediaPaths.GetMediaFolder(artworkId, mediaId);
        var extension = Path.GetExtension(request.FileName).ToLowerInvariant();

        var originalPath = $"{folder}/original{extension}";
        var thumbnailPath = $"{folder}/thumbnail.webp";
        var previewPath = $"{folder}/preview.webp";

        try
        {
            await _fileStorage.SaveAsync(request.Content, originalPath, cancellationToken);

            ImageProcessingResult processed;
            await using (var original = await _fileStorage.OpenReadAsync(originalPath, cancellationToken))
            {
                processed = await _imageProcessor.ProcessAsync(original, cancellationToken);
            }

            await _fileStorage.SaveAsync(new MemoryStream(processed.Thumbnail), thumbnailPath, cancellationToken);

            await _fileStorage.SaveAsync(new MemoryStream(processed.Preview), previewPath, cancellationToken);

            var media = new MediaFile
            {
                Id = mediaId,
                ArtworkId = artworkId,
                OriginalFileName = Path.GetFileName(request.FileName),
                StoragePath = originalPath,
                ThumbnailPath = thumbnailPath,
                PreviewPath = previewPath,
                ContentType = MediaRules.AllowedExtensions[extension],
                FileSize = request.Length,
                Width = processed.Width,
                Height = processed.Height,
                SortOrder = await _mediaRepository.GetNextSortOrderAsync(artworkId)
            };

            await _mediaRepository.AddAsync(media);

            return media.ToResponse();
        }
        catch
        {
            try
            {
                await _fileStorage.DeleteFolderAsync(folder);
            }
            catch (Exception cleanupEx)
            {
                _logger.LogWarning(cleanupEx, "Could not clean up folder {Folder} after a failed upload.", folder);
            }

            throw;
        }
    }

    public async Task<List<MediaFileResponse>> GetByArtworkAsync(
    Guid artworkId, CancellationToken cancellationToken = default)
    {
        if (!await _artworkRepository.ExistsAsync(artworkId))
            throw new ArtworkNotFoundException(artworkId);

        var mediaFiles = await _mediaRepository.GetByArtworkIdAsync(artworkId);

        return mediaFiles.Select(m => m.ToResponse()).ToList();
    }

    public async Task<MediaContent> GetContentAsync(
        Guid artworkId, Guid mediaId, MediaVariant variant, CancellationToken cancellationToken = default)
    {
        var media = await _mediaRepository.GetByIdAsync(mediaId);

        if (media is null || media.ArtworkId != artworkId)
            throw new MediaFileNotFoundException(mediaId);

        var (path, contentType, fileName) = variant switch
        {
            MediaVariant.Original => (media.StoragePath, media.ContentType, media.OriginalFileName),
            MediaVariant.Preview => (media.PreviewPath, "image/webp", $"{Path.GetFileNameWithoutExtension(media.OriginalFileName)}-preview.webp"),
            MediaVariant.Thumbnail => (media.ThumbnailPath, "image/webp", $"{Path.GetFileNameWithoutExtension(media.OriginalFileName)}-thumbnail.webp"),
            _ => throw new MediaFileNotFoundException(mediaId)
        };

        var stream = await _fileStorage.OpenReadAsync(path, cancellationToken);

        return new MediaContent(stream, contentType, fileName);
    }

    public async Task DeleteAsync(Guid artworkId, Guid mediaId, CancellationToken cancellationToken = default)
    {
        var media = await _mediaRepository.GetByIdAsync(mediaId);

        if (media is null || media.ArtworkId != artworkId)
            throw new MediaFileNotFoundException(mediaId);

        // 1. Primero la base de datos (reversible si falla: no se tocó nada)
        await _mediaRepository.DeleteAsync(media);

        // 2. Después el disco (si falla, quedan archivos huérfanos, pero no se pierde nada)
        var folder = MediaPaths.GetMediaFolder(artworkId, mediaId);

        try
        {
            await _fileStorage.DeleteFolderAsync(folder);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Media {MediaId} was deleted from the database but its folder {Folder} could not be removed.",
                mediaId, folder);
        }
    }

    public async Task<List<MediaFileResponse>> ReorderAsync(
    Guid artworkId, ReorderMediaRequest request, CancellationToken cancellationToken = default)
    {
        await _reorderValidator.ValidateAndThrowAsync(request, cancellationToken);

        if (!await _artworkRepository.ExistsAsync(artworkId))
            throw new ArtworkNotFoundException(artworkId);

        var mediaFiles = await _mediaRepository.GetByArtworkIdForUpdateAsync(artworkId);

        var existingIds = mediaFiles.Select(m => m.Id).ToHashSet();

        if (!existingIds.SetEquals(request.MediaIds))
            throw new InvalidMediaOrderException(
                "The list must contain exactly all media files of the artwork.");

        var byId = mediaFiles.ToDictionary(m => m.Id);

        for (var i = 0; i < request.MediaIds.Count; i++)
            byId[request.MediaIds[i]].SortOrder = i + 1;

        await _mediaRepository.UpdateRangeAsync(mediaFiles);

        return mediaFiles
            .OrderBy(m => m.SortOrder)
            .Select(m => m.ToResponse())
            .ToList();
    }
}
