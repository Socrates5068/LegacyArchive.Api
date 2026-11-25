namespace LegacyArchive.Application.Models;

public record ImageProcessingResult(int Width, int Height, byte[] Thumbnail, byte[] Preview);
