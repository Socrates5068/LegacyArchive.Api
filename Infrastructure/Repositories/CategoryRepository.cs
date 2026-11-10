using LegacyArchive.Application.Interfaces;
using LegacyArchive.Domain.Entities;
using LegacyArchive.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LegacyArchive.Infrastructure.Repositories;

public class CategoryRepository : ICategoryRepository
{
    private readonly AppDbContext _context;

    public CategoryRepository(
        AppDbContext context)
    {
        _context = context;
    }

    public async Task<Category> AddAsync(
        Category category)
    {
        _context.Categories.Add(category);

        await _context.SaveChangesAsync();

        return category;
    }

    public async Task<Category?> GetByIdAsync(
        Guid id)
    {
        return await _context.Categories
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<List<Category>> GetAllAsync()
    {
        return await _context.Categories
            .ToListAsync();
    }

    public async Task<bool> NameExistsAsync(string name, Guid? excludeId = null)
    {
        return await _context.Categories
            .AnyAsync(c => c.Name.ToLower() == name.ToLower()
                        && c.Id != excludeId);
    }

    public async Task UpdateAsync(
        Category category)
    {
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(
        Category category)
    {
        _context.Categories.Remove(category);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> HasArtworksAsync(Guid categoryId)
    {
        return await _context.Artworks
            .AnyAsync(a => a.CategoryId == categoryId);
    }
}
