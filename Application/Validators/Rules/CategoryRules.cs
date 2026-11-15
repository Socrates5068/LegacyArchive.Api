using FluentValidation;

namespace LegacyArchive.Application.Validators.Rules;

public static class CategoryRules
{
    public const int NameMinLength = 3;
    public const int NameMaxLength = 100;

    public static IRuleBuilderOptions<T, string> ValidCategoryName<T>(
        this IRuleBuilder<T, string> rule)
    {
        return rule
            .NotEmpty()
            .Must(name => name.Trim().Length >= NameMinLength)
                .WithMessage($"Name must have at least {NameMinLength} characters.")
            .Must(name => name.Trim().Length <= NameMaxLength)
                .WithMessage($"Name must have at most {NameMaxLength} characters.");
    }
}
