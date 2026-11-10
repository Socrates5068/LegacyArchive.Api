namespace LegacyArchive.Application.Exceptions;

public class ArtworkNotFoundException : Exception
{
    public ArtworkNotFoundException(Guid artworkId)
        : base($"Artwork '{artworkId}' not found.")
    {
    }
}
