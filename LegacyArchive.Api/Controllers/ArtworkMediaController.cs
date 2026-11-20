using LegacyArchive.Application.DTOs;
using LegacyArchive.Application.Interfaces;
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

        return Created($"/api/artworks/{artworkId}/media/{media.Id}", media);
    }
}
