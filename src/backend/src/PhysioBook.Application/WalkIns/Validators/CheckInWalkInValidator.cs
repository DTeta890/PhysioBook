using FluentValidation;
using PhysioBook.Application.WalkIns.Commands;

namespace PhysioBook.Application.WalkIns.Validators;

public sealed class CheckInWalkInValidator : AbstractValidator<CheckInWalkInCommand>
{
    public CheckInWalkInValidator()
    {
        RuleFor(x => x.PatientName)
            .NotEmpty().WithMessage("PatientName is required.")
            .MaximumLength(200).WithMessage("PatientName must not exceed 200 characters.");

        RuleFor(x => x.PatientPhone)
            .MaximumLength(50).WithMessage("PatientPhone must not exceed 50 characters.");

        RuleFor(x => x.Priority)
            .Must(p => p is null or 1 or 2)
            .WithMessage("Priority must be 1 (Normal) or 2 (Urgent).");

        RuleFor(x => x.ReasonForVisit)
            .MaximumLength(500).WithMessage("ReasonForVisit must not exceed 500 characters.");

        RuleFor(x => x.Notes)
            .MaximumLength(1000).WithMessage("Notes must not exceed 1000 characters.");
    }
}
