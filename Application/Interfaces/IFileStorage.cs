namespace LegacyArchive.Application.Interfaces;

public interface IFileStorage
{
    Task SaveAsync(Stream content, string relativePath, CancellationToken cancellationToken = default);

    Task<Stream> OpenReadAsync(string relativePath, CancellationToken cancellationToken = default);

    Task DeleteFolderAsync(string relativePath);
}