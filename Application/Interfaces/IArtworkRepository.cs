using LegacyArchive.Domain.Entities;

namespace Application.Interfaces;

public interface IArtworkRepository
{
    Task<Artwork> AddAsync(Artwork artwork);

    Task<Artwork?> GetByIdAsync(Guid id);

    Task<List<Artwork>> GetAllAsync();
}
