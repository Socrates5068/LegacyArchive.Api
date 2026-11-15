using FluentValidation;
using LegacyArchive.Application.DTOs;
using LegacyArchive.Application.Validators.Rules;

namespace LegacyArchive.Application.Validators;

public class UpdateArtworkRequestValidator : AbstractValidator<UpdateArtworkRequest>
{
    public UpdateArtworkRequestValidator()
    {
        RuleFor(x => x.Title).Cascade(CascadeMode.Stop).ValidTitle();
        RuleFor(x => x.Description).Cascade(CascadeMode.Stop).ValidDescription();
        RuleFor(x => x.Year).Cascade(CascadeMode.Stop).ValidYear();
        RuleFor(x => x.CategoryId).Cascade(CascadeMode.Stop).ValidCategoryId();
    }
}
