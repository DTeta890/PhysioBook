using FluentValidation;
using PhysioBook.Application.Appointments.Queries;

namespace PhysioBook.Application.Appointments.Validators;

public sealed class CheckConflictsValidator : AbstractValidator<CheckConflictsQuery>
{
    public CheckConflictsValidator()
    {
        RuleFor(x => x.TherapistId)
            .NotEmpty().WithMessage("TherapistId is required.");

        RuleFor(x => x.StartTime)
            .NotEmpty().WithMessage("StartTime is required.");

        RuleFor(x => x.EndTime)
            .NotEmpty().WithMessage("EndTime is required.")
            .GreaterThan(x => x.StartTime).WithMessage("EndTime must be after StartTime.");

        RuleFor(x => x.StartTime)
            .Must(startTime => startTime >= new DateTimeOffset(DateTimeOffset.UtcNow.Date, TimeSpan.Zero))
            .WithMessage("StartTime must not be in the far past.");
    }
}
