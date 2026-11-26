namespace LegacyArchive.Application.Storage;

public static class MediaPaths
{
    public static string GetArtworkFolder(Guid artworkId)
        => $"artworks/{artworkId}";

    public static string GetMediaFolder(Guid artworkId, Guid mediaId)
        => $"{GetArtworkFolder(artworkId)}/{mediaId}";
}