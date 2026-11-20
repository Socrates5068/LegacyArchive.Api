namespace LegacyArchive.Application.Interfaces;

public interface IFileStorage
{
    Task SaveAsync(Stream content, string relativePath, CancellationToken cancellationToken = default);
    Task DeleteAsync(string relativePath);
}