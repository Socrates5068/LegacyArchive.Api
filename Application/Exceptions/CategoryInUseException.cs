namespace LegacyArchive.Application.Exceptions;

public class CategoryInUseException : Exception
{
    public CategoryInUseException(Guid categoryId)
        : base($"Category '{categoryId}' has artworks and cannot be deleted.")
    {
    }
}
