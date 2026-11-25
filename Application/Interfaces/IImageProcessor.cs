using LegacyArchive.Application.Models;

namespace LegacyArchive.Application.Interfaces;

public interface IImageProcessor
{
    Task<ImageProcessingResult> ProcessAsync(Stream source, CancellationToken cancellationToken = default);
}
