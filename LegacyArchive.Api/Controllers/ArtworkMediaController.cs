using LegacyArchive.Application.DTOs;
using LegacyArchive.Application.Interfaces;
using LegacyArchive.Application.Models;
using LegacyArchive.Application.Validators.Rules;
using Microsoft.AspNetCore.Mvc;

namespace LegacyArchive.Api.Controllers;

[ApiController]
[Route("api/artworks/{artworkId:guid}/media")]
public class ArtworkMediaController : ControllerBase
{
    private readonly IMediaService _mediaService;

    public ArtworkMediaController(IMediaService mediaService)
    {
        _mediaService = mediaService;
    }

    [HttpPost]
    [RequestSizeLimit(MediaRules.MaxFileSizeBytes + 1024 * 1024)]
    [RequestFormLimits(MultipartBodyLengthLimit = MediaRules.MaxFileSizeBytes + 1024 * 1024)]
    public async Task<IActionResult> Upload(
        Guid artworkId, IFormFile file, CancellationToken cancellationToken)
    {
        await using var stream = file.OpenReadStream();

        var request = new UploadMediaRequest
        {
            Content = stream,
            FileName = file.FileName,
            Length = file.Length
        };

        var media = await _mediaService.UploadAsync(artworkId, request, cancellationToken);

        return Created(media.OriginalUrl, media);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(Guid artworkId, CancellationToken cancellationToken)
    {
        var media = await _mediaService.GetByArtworkAsync(artworkId, cancellationToken);
        return Ok(media);
    }

    [HttpGet("{mediaId:guid}/{variant}")]
    public async Task<IActionResult> GetContent(
        Guid artworkId, Guid mediaId, MediaVariant variant, CancellationToken cancellationToken)
    {
        var content = await _mediaService.GetContentAsync(artworkId, mediaId, variant, cancellationToken);

        Response.Headers.CacheControl = "public, max-age=31536000, immutable";

        return variant == MediaVariant.Original
            ? File(content.Content, content.ContentType, content.FileName, enableRangeProcessing: true)
            : File(content.Content, content.ContentType, enableRangeProcessing: true);
    }

    [HttpDelete("{mediaId:guid}")]
    public async Task<IActionResult> Delete(Guid artworkId, Guid mediaId, CancellationToken cancellationToken)
    {
        await _mediaService.DeleteAsync(artworkId, mediaId, cancellationToken);
        return NoContent();
    }

    [HttpPut("order")]
    public async Task<IActionResult> Reorder(
    Guid artworkId, ReorderMediaRequest request, CancellationToken cancellationToken)
    {
        var media = await _mediaService.ReorderAsync(artworkId, request, cancellationToken);
        return Ok(media);
    }
}
