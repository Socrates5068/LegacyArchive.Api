using LegacyArchive.Application.DTOs;
using LegacyArchive.Application.DTOs.Responses;

namespace LegacyArchive.Application.Interfaces;

public interface ICategoryService
{
    Task<CategoryResponse> CreateAsync(
        CreateCategoryRequest request);

    Task<CategoryResponse?> GetByIdAsync(Guid id);

    Task<List<CategoryResponse>> GetAllAsync();

    Task<CategoryResponse> UpdateAsync(
        Guid id, UpdateCategoryRequest request);

    Task DeleteAsync(Guid id);
}
