namespace LegacyArchive.Application.Exceptions;

public class MediaFileNotFoundException : Exception
{
    public MediaFileNotFoundException(Guid mediaId)
        : base($"Media '{mediaId}' not found.")
    {
    }
}
