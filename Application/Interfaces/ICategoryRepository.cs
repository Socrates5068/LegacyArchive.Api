namespace Application.Interfaces;

public interface ICategoryRepository
{
    Task<bool> ExistsAsync(Guid categoryId);
}
