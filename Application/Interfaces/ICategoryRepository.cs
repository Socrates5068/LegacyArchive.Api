using LegacyArchive.Domain.Entities;

namespace LegacyArchive.Application.Interfaces;

public interface ICategoryRepository
{
    Task<Category> AddAsync(Category category);

    Task<Category?> GetByIdAsync(Guid id);

    Task<List<Category>> GetAllAsync();

    Task<bool> NameExistsAsync(string name, Guid? excludeId = null);

    Task UpdateAsync(Category category);

    Task DeleteAsync(Category category);

    Task<bool> HasArtworksAsync(Guid categoryId);
}
