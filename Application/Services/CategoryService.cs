using LegacyArchive.Application.DTOs;
using LegacyArchive.Application.DTOs.Responses;
using LegacyArchive.Application.Exceptions;
using LegacyArchive.Application.Interfaces;
using LegacyArchive.Application.Mappings;
using LegacyArchive.Domain.Entities;

namespace LegacyArchive.Application.Services;

public class CategoryService
    : ICategoryService
{
    private readonly ICategoryRepository _repository;

    public CategoryService(
        ICategoryRepository repository)
    {
        _repository = repository;
    }

    public async Task<CategoryResponse> CreateAsync(
        CreateCategoryRequest request)
    {
        var categoryName = request.Name.Trim();

        if (string.IsNullOrWhiteSpace(categoryName))
        {
            throw new InvalidOperationException(
                "Category name cannot be empty.");
        }

        if (await _repository.NameExistsAsync(categoryName))
        {
            throw new CategoryAlreadyExistsException(
                categoryName);
        }

        var category = new Category
        {
            Name = categoryName
        };

        var created =
            await _repository.AddAsync(category);

        return created.ToResponse();
    }

    public async Task<CategoryResponse?> GetByIdAsync(
        Guid id)
    {
        var category = await _repository.GetByIdAsync(id);

        return category?.ToResponse();
    }

    public async Task<List<CategoryResponse>> GetAllAsync()
    {
        var categories =
            await _repository.GetAllAsync();

        return categories.Select(c => c.ToResponse()).ToList();
    }

    public async Task<CategoryResponse> UpdateAsync(
        Guid id, UpdateCategoryRequest request)
    {
        var category = await _repository.GetByIdAsync(id)
            ?? throw new CategoryNotFoundException(id);

        var newName = request.Name.Trim();

        if (string.IsNullOrWhiteSpace(newName))
        {
            throw new InvalidOperationException(
                "Category name cannot be empty.");
        }

        if (await _repository.NameExistsAsync(newName, id))
            throw new CategoryAlreadyExistsException(newName);

        category.Name = newName;

        await _repository.UpdateAsync(category);

        return category.ToResponse();
    }

    public async Task DeleteAsync(
        Guid id)
    {
        var category = await _repository.GetByIdAsync(id)
            ?? throw new CategoryNotFoundException(id);

        if (await _repository.HasArtworksAsync(id))
            throw new CategoryInUseException(id);

        await _repository.DeleteAsync(category);
    }
}
