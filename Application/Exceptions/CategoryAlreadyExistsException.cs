namespace LegacyArchive.Application.Exceptions;

public class CategoryAlreadyExistsException : Exception
{
    public CategoryAlreadyExistsException(string name)
        : base($"Category '{name}' already exists.")
    {
    }
}
