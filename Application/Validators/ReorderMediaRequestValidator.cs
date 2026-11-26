using FluentValidation;
using LegacyArchive.Application.DTOs;

namespace LegacyArchive.Application.Validators;

public class ReorderMediaRequestValidator : AbstractValidator<ReorderMediaRequest>
{
    public ReorderMediaRequestValidator()
    {
        RuleFor(x => x.MediaIds)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
                .WithMessage("The list of media ids cannot be empty.")
            .Must(ids => ids.Distinct().Count() == ids.Count)
                .WithMessage("The list of media ids cannot contain duplicates.");
    }
}
