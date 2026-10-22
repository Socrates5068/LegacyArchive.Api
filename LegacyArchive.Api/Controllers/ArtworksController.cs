using Microsoft.AspNetCore.Mvc;
using LegacyArchive.Domain.Entities;

namespace LegacyArchive.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ArtworksController : ControllerBase
{
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
}