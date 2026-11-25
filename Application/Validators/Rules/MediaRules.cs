using FluentValidation;

namespace LegacyArchive.Application.Validators.Rules;

public static class MediaRules
{
    public const int MaxFileSizeMb = 100;
    public const long MaxFileSizeBytes = MaxFileSizeMb * 1024L * 1024L;
    public const int ThumbnailMaxSize = 400;
    public const int PreviewMaxSize = 1600;
    public const long MaxPixels = 150_000_000;

    public static readonly IReadOnlyDictionary<string, string> AllowedExtensions =
        new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            [".jpg"] = "image/jpeg",
            [".jpeg"] = "image/jpeg",
            [".png"] = "image/png",
            [".tif"] = "image/tiff",
            [".tiff"] = "image/tiff",
            [".webp"] = "image/webp",
        };

    public static IRuleBuilderOptions<T, long> ValidLength<T>(
        this IRuleBuilder<T, long> rule)
    {
        return rule
            .GreaterThan(0)
                .WithMessage("The uploaded file is empty.")
            .LessThanOrEqualTo(MaxFileSizeBytes)
                .WithMessage((_, fileSize) =>
                    $"File size ({fileSize / (1024.0 * 1024.0):F2} MB) exceeds the maximum allowed limit of {MaxFileSizeMb} MB.");
    }

    public static IRuleBuilderOptions<T, string> ValidExtension<T>(
        this IRuleBuilder<T, string> rule)
    {
        return rule
        .NotEmpty()
            .WithMessage("File name is required.")
        .Must(fileName =>
        {
            var extension = Path.GetExtension(fileName);
            return !string.IsNullOrWhiteSpace(extension) && AllowedExtensions.ContainsKey(extension);
        })
        .WithMessage($"File extension must be one of the following: {string.Join(", ", AllowedExtensions.Keys)}");
    }
}
