using FluentValidation;
using LegacyArchive.Application.DTOs;
using LegacyArchive.Application.Validators.Rules;

namespace LegacyArchive.Application.Validators;

public class UploadMediaRequestValidator : AbstractValidator<UploadMediaRequest>
{
    public UploadMediaRequestValidator()
    {
        RuleFor(x => x.Length).Cascade(CascadeMode.Stop).ValidLength();
        RuleFor(x => x.FileName).Cascade(CascadeMode.Stop).ValidExtension();
    }
}
