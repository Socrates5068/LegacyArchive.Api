namespace Application.Exceptions;

public class CategoryNotFoundException : Exception
{
    public CategoryNotFoundException(Guid categoryId)
        : base($"Category '{categoryId}' not found.")
    {
    }

    public sealed class CategoryAlreadyExistsException
    : Exception
    {
        public CategoryAlreadyExistsException(string name)
            : base($"Category '{name}' already exists.")
        {
        }
    }
}
