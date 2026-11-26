namespace LegacyArchive.Application.Exceptions;

public class InvalidMediaOrderException : Exception
{
    public InvalidMediaOrderException(string message)
        : base(message)
    {
    }
}