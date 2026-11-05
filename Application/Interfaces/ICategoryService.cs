using Application.DTOs;
using Application.DTOs.Responses;

namespace Application.Interfaces;

public interface ICategoryService
{
    Task<CategoryResponse> CreateAsync(
        CreateCategoryRequest request);

    Task<CategoryResponse?> GetByIdAsync(Guid id);

    Task<List<CategoryResponse>> GetAllAsync();
}
