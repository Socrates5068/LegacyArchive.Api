using LegacyArchive.Domain.Entities;

namespace LegacyArchive.Application.Interfaces;

public interface IArtworkRepository
{
    Task<Artwork> AddAsync(Artwork artwork);

    Task<Artwork?> GetByIdAsync(Guid id);

    Task<List<Artwork>> GetAllAsync();
    Task UpdateAsync(Artwork artwork);
    Task DeleteAsync(Artwork artwork);
}
