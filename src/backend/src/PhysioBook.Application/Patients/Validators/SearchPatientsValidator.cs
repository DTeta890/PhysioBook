using FluentValidation;
using PhysioBook.Application.Patients.Queries;

namespace PhysioBook.Application.Patients.Validators;

public sealed class SearchPatientsValidator : AbstractValidator<SearchPatientsQuery>
{
    public SearchPatientsValidator()
    {
        RuleFor(x => x.SearchTerm)
            .NotEmpty().WithMessage("SearchTerm is required.")
            .MinimumLength(1).WithMessage("SearchTerm must be at least 1 character.")
            .MaximumLength(100).WithMessage("SearchTerm must not exceed 100 characters.");

        RuleFor(x => x.Limit)
            .InclusiveBetween(1, 50).WithMessage("Limit must be between 1 and 50.");
    }
}
