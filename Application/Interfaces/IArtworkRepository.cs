using LegacyArchive.Domain.Entities;

namespace LegacyArchive.Application.Interfaces;

public interface IArtworkRepository
{
    Task<Artwork> AddAsync(Artwork artwork);

    Task<Artwork?> GetByIdAsync(Guid id);

    Task<(List<Artwork> Items, int TotalCount)> GetPagedAsync(int page, int pageSize);

    Task UpdateAsync(Artwork artwork);

    Task DeleteAsync(Artwork artwork);

    Task<bool> ExistsAsync(Guid id);
}
