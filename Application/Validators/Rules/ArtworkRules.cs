using FluentValidation;

namespace LegacyArchive.Application.Validators.Rules; 

public static class ArtworkRules
{
    public const int TitleMaxLength = 200;
    public const int DescriptionMaxLength = 2000;
    public const int YearMinValue = 1950;


    public static IRuleBuilderOptions<T, string> ValidTitle<T>(
        this IRuleBuilder<T, string> rule)
    {
        return rule
            .NotEmpty()
            .Must(title => title.Trim().Length <= TitleMaxLength)
                .WithMessage($"Title must have at most {TitleMaxLength} characters.");
    }

    public static IRuleBuilderOptions<T, string?> ValidDescription<T>(
        this IRuleBuilder<T, string?> rule)
    {
        return rule
            .MaximumLength(DescriptionMaxLength)
                .WithMessage($"Description must have at most {DescriptionMaxLength} characters.");
    }

    public static IRuleBuilderOptions<T, int?> ValidYear<T>(
    this IRuleBuilder<T, int?> rule)
    {
        return rule
            .Must(year => year is null || (year >= YearMinValue && year <= DateTime.UtcNow.Year))
                .WithMessage($"Year must be between {YearMinValue} and the current year.");
    }

    public static IRuleBuilderOptions<T, Guid> ValidCategoryId<T>(
        this IRuleBuilder<T, Guid> rule)
    {
        return rule
            .NotEmpty()
                .WithMessage("CategoryId is required.");
    }
}
