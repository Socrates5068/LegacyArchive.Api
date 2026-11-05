using Application.DTOs;
using Application.DTOs.Responses;
using Application.Interfaces;
using LegacyArchive.Domain.Entities;

using static Application.Exceptions.CategoryNotFoundException;

namespace Application.Services;

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
        var exists =
            await _repository.NameExistsAsync(
                request.Name);

        if (exists)
        {
            throw new CategoryAlreadyExistsException(
                request.Name);
        }

        var category = new Category
        {
            Name = request.Name
        };

        var created =
            await _repository.AddAsync(category);

        return new CategoryResponse
        {
            Id = created.Id,
            Name = created.Name
        };
    }

    public async Task<CategoryResponse?> GetByIdAsync(
        Guid id)
    {
        var category =
            await _repository.GetByIdAsync(id);

        if (category is null)
            return null;

        return new CategoryResponse
        {
            Id = category.Id,
            Name = category.Name
        };
    }

    public async Task<List<CategoryResponse>> GetAllAsync()
    {
        var categories =
            await _repository.GetAllAsync();

        return categories
            .Select(c => new CategoryResponse
            {
                Id = c.Id,
                Name = c.Name
            })
            .ToList();
    }
}
