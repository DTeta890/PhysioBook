using FluentValidation;
using PhysioBook.Application.RecurringRules.Commands;

namespace PhysioBook.Application.RecurringRules.Validators;

public sealed class GenerateAppointmentsValidator : AbstractValidator<GenerateAppointmentsCommand>
{
    public GenerateAppointmentsValidator()
    {
        RuleFor(x => x.RecurringRuleId)
            .NotEmpty().WithMessage("Recurring rule ID is required.");

        RuleFor(x => x.FromDate)
            .NotEmpty().WithMessage("From date is required.");

        RuleFor(x => x.ToDate)
            .NotEmpty().WithMessage("To date is required.")
            .GreaterThan(x => x.FromDate).WithMessage("To date must be after from date.");

        RuleFor(x => x)
            .Must(x => (x.ToDate - x.FromDate).TotalDays <= 366)
            .WithMessage("Date range must not exceed 366 days.");
    }
}
