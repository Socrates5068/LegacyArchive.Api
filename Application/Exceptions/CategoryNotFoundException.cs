namespace LegacyArchive.Application.Exceptions;

public class CategoryNotFoundException : Exception
{
    public CategoryNotFoundException(Guid categoryId)
        : base($"Category '{categoryId}' not found.")
    {
    }
}
