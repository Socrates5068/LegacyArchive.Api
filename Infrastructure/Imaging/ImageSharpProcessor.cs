using LegacyArchive.Application.Exceptions;
using LegacyArchive.Application.Interfaces;
using LegacyArchive.Application.Models;
using LegacyArchive.Application.Validators.Rules;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.Processing;

namespace LegacyArchive.Infrastructure.Imaging;

public class ImageSharpProcessor : IImageProcessor
{
    private static readonly WebpEncoder Encoder = new() { Quality = 80 };

    public async Task<ImageProcessingResult> ProcessAsync(
        Stream source, CancellationToken cancellationToken = default)
    {
        try
        {
            // 1. Leer SOLO el encabezado: dimensiones y formato, sin decodificar los píxeles
            var info = await Image.IdentifyAsync(source, cancellationToken);

            if ((long)info.Width * info.Height > MediaRules.MaxPixels)
                throw new InvalidImageException(
                    $"Image is too large ({info.Width}x{info.Height} px).");

            source.Position = 0;

            // 2. Ahora sí, cargar la imagen completa
            using var image = await Image.LoadAsync(source, cancellationToken);
            image.Mutate(x => x.AutoOrient());

            // 3. Generar las dos versiones desde la MISMA imagen en memoria
            var thumbnail = await ResizeToWebpAsync(image, MediaRules.ThumbnailMaxSize, cancellationToken);
            var preview = await ResizeToWebpAsync(image, MediaRules.PreviewMaxSize, cancellationToken);

            return new ImageProcessingResult(image.Width, image.Height, thumbnail, preview);
        }
        catch (ImageFormatException)
        {
            throw new InvalidImageException("The file is not a valid image.");
        }
    }

    private static async Task<byte[]> ResizeToWebpAsync(
        Image image, int maxSize, CancellationToken cancellationToken)
    {
        var longestSide = Math.Max(image.Width, image.Height);
        var targetSize = Math.Min(maxSize, longestSide);   // nunca agrandar

        using var resized = image.Clone(x => x.Resize(new ResizeOptions
        {
            Mode = ResizeMode.Max,
            Size = new Size(targetSize, targetSize)
        }));

        using var output = new MemoryStream();
        await resized.SaveAsWebpAsync(output, Encoder, cancellationToken);
        return output.ToArray();
    }
}
