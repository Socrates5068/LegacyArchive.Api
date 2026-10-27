using Application.DTOs;
using Application.Services;
using LegacyArchive.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace LegacyArchive.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ArtworksController : ControllerBase
{
    private readonly ArtworkService _artworkService;

    public ArtworksController(ArtworkService artworkService)
    {
        _artworkService = artworkService;
    }

    [HttpGet]
    public IActionResult Get()
    {
        var artwork = new Artwork
        {
            Id = Guid.NewGuid(),
            Title = "El Primer Comic",
            Description = "Prueba del sistema",
            Year = 2026,
            CreatedAt = DateTime.UtcNow
        };

        return Ok(artwork);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var artwork = await _artworkService.GetByIdAsync(id);

        if (artwork is null)
            return NotFound();

        return Ok(artwork);
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        CreateArtworkRequest request)
    {
        var createdArtwork = await _artworkService.CreateAsync(request);

        return Ok(createdArtwork);
    }
}