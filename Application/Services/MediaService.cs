using FluentValidation;
using LegacyArchive.Application.DTOs;
using LegacyArchive.Application.DTOs.Responses;
using LegacyArchive.Application.Exceptions;
using LegacyArchive.Application.Interfaces;
using LegacyArchive.Application.Mappings;
using LegacyArchive.Application.Models;
using LegacyArchive.Application.Validators.Rules;
using LegacyArchive.Domain.Entities;

namespace LegacyArchive.Application.Services;

public class MediaService : IMediaService
{
    private readonly IValidator<UploadMediaRequest> _validator;
    private readonly IArtworkRepository _artworkRepository;
    private readonly IFileStorage _fileStorage;
    private readonly IMediaFileRepository _mediaRepository;
    private readonly IImageProcessor _imageProcessor;

    public MediaService(IValidator<UploadMediaRequest> validator,
        IArtworkRepository artworkRepository,
        IFileStorage fileStorage,
        IMediaFileRepository mediaRepository,
        IImageProcessor imageProcessor)
    {
        _validator = validator;
        _artworkRepository = artworkRepository;
        _fileStorage = fileStorage;
        _mediaRepository = mediaRepository;
        _imageProcessor = imageProcessor;   
    }

    public async Task<MediaFileResponse> UploadAsync(
    Guid artworkId, UploadMediaRequest request, CancellationToken cancellationToken = default)
    {
        await _validator.ValidateAndThrowAsync(request, cancellationToken);

        if (!await _artworkRepository.ExistsAsync(artworkId))
            throw new ArtworkNotFoundException(artworkId);

        var mediaId = Guid.NewGuid();
        var folder = $"artworks/{artworkId}/{mediaId}";
        var extension = Path.GetExtension(request.FileName).ToLowerInvariant();

        var originalPath = $"{folder}/original{extension}";
        var thumbnailPath = $"{folder}/thumbnail.webp";
        var previewPath = $"{folder}/preview.webp";

        var savedPaths = new List<string>();

        try
        {
            savedPaths.Add(originalPath);
            await _fileStorage.SaveAsync(request.Content, originalPath, cancellationToken);

            ImageProcessingResult processed;
            await using (var original = await _fileStorage.OpenReadAsync(originalPath, cancellationToken))
            {
                processed = await _imageProcessor.ProcessAsync(original, cancellationToken);
            }

            savedPaths.Add(thumbnailPath);
            await _fileStorage.SaveAsync(new MemoryStream(processed.Thumbnail), thumbnailPath, cancellationToken);

            savedPaths.Add(previewPath);
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
            foreach (var path in savedPaths)
                await _fileStorage.DeleteAsync(path);

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
}
