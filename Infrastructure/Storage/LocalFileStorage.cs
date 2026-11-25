using LegacyArchive.Application.Interfaces;
using Microsoft.Extensions.Options;

namespace LegacyArchive.Infrastructure.Storage;

public class LocalFileStorage : IFileStorage
{
    private readonly string _rootPath;

    public LocalFileStorage(IOptions<FileStorageOptions> options)
    {
        if (string.IsNullOrWhiteSpace(options.Value.RootPath))
            throw new InvalidOperationException("FileStorage:RootPath is not configured.");

        _rootPath = Path.GetFullPath(options.Value.RootPath);
    }

    public async Task SaveAsync(Stream content, string relativePath, CancellationToken cancellationToken = default)
    {
        var fullPath = GetSafeFullPath(relativePath);

        Directory.CreateDirectory(Path.GetDirectoryName(fullPath)!);

        await using var fileStream = new FileStream(fullPath, FileMode.CreateNew);
        await content.CopyToAsync(fileStream, cancellationToken);
    }

    public Task DeleteAsync(string relativePath)
    {
        var fullPath = GetSafeFullPath(relativePath);

        if (File.Exists(fullPath))
            File.Delete(fullPath);

        return Task.CompletedTask;
    }

    private string GetSafeFullPath(string relativePath)
    {
        var fullPath = Path.GetFullPath(Path.Combine(_rootPath, relativePath));

        if (!fullPath.StartsWith(_rootPath + Path.DirectorySeparatorChar))
            throw new InvalidOperationException("Invalid storage path.");

        return fullPath;
    }

    public Task<Stream> OpenReadAsync(string relativePath, CancellationToken cancellationToken = default)
    {
        var fullPath = GetSafeFullPath(relativePath);

        Stream stream = new FileStream(
            fullPath, FileMode.Open, FileAccess.Read, FileShare.Read,
            bufferSize: 81920, useAsync: true);

        return Task.FromResult(stream);
    }
}
