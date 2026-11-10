using LegacyArchive.Application.DTOs;
using LegacyArchive.Application.Interfaces;
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

        return CreatedAtAction(
        nameof(GetById),
        new { id = createdArtwork.Id },
        createdArtwork);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var artworks =
            await _artworkService.GetAllAsync();

        return Ok(artworks);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, UpdateArtworkRequest request)
    {
        var updated = await _artworkService.UpdateAsync(id, request);
        return Ok(updated);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _artworkService.DeleteAsync(id);
        return NoContent();
    }
}