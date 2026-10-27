using Application.DTOs;
using Application.Interfaces;
using Application.Services;
using LegacyArchive.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace LegacyArchive.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ArtworksController : ControllerBase
{
    private readonly IArtworkService _artworkService;

    public ArtworksController(IArtworkService artworkService)
    {
        _artworkService = artworkService;
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

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var artworks =
            await _artworkService.GetAllAsync();

        return Ok(artworks);
    }
}