using FluentValidation;
using LegacyArchive.Application.DTOs;

namespace LegacyArchive.Application.Validators;

public class PaginationQueryValidator : AbstractValidator<PaginationQuery>
{
    public PaginationQueryValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThanOrEqualTo(1);

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, PaginationQuery.MaxPageSize);
    }
}
