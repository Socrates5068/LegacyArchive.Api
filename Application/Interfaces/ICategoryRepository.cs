using LegacyArchive.Domain.Entities;

namespace LegacyArchive.Application.Interfaces;

public interface ICategoryRepository
{
    Task<bool> ExistsAsync(Guid categoryId);

    Task<Category> AddAsync(Category category);

    Task<Category?> GetByIdAsync(Guid id);

    Task<List<Category>> GetAllAsync();

    Task<bool> NameExistsAsync(string name);
}
