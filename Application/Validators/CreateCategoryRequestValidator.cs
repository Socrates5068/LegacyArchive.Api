using FluentValidation;
using LegacyArchive.Application.DTOs;
using LegacyArchive.Application.Validators.Rules;

namespace LegacyArchive.Application.Validators;

public class CreateCategoryRequestValidator : AbstractValidator<CreateCategoryRequest>
{
    public CreateCategoryRequestValidator()
    {
        RuleFor(x => x.Name).Cascade(CascadeMode.Stop).ValidCategoryName();
    }
}
