using FluentValidation;
using LegacyArchive.Application.DTOs;
using LegacyArchive.Application.Validators.Rules;

namespace LegacyArchive.Application.Validators;

public class UpdateCategoryRequestValidator : AbstractValidator<UpdateCategoryRequest>
{
    public UpdateCategoryRequestValidator()
    {
        RuleFor(x => x.Name).Cascade(CascadeMode.Stop).ValidCategoryName();
    }
}
