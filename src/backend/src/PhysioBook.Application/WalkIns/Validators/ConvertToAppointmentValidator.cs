using FluentValidation;
using PhysioBook.Application.WalkIns.Commands;

namespace PhysioBook.Application.WalkIns.Validators;

public sealed class ConvertToAppointmentValidator : AbstractValidator<ConvertToAppointmentCommand>
{
    public ConvertToAppointmentValidator()
    {
        RuleFor(x => x.WalkInId)
            .NotEmpty().WithMessage("WalkInId is required.");

        RuleFor(x => x.TherapistId)
            .NotEmpty().WithMessage("TherapistId is required.");

        RuleFor(x => x.TreatmentTypeId)
            .NotEmpty().WithMessage("TreatmentTypeId is required.");

        RuleFor(x => x.StartTime)
            .NotEmpty().WithMessage("StartTime is required.");

        RuleFor(x => x.EndTime)
            .NotEmpty().WithMessage("EndTime is required.")
            .GreaterThan(x => x.StartTime).WithMessage("EndTime must be after StartTime.");
    }
}
