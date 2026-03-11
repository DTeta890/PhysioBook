using FluentValidation;
using PhysioBook.Application.Appointments.Commands;

namespace PhysioBook.Application.Appointments.Validators;

public sealed class UpdateAppointmentValidator : AbstractValidator<UpdateAppointmentCommand>
{
    public UpdateAppointmentValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Appointment Id is required.");

        RuleFor(x => x.TherapistId)
            .NotEmpty().WithMessage("TherapistId is required.");

        RuleFor(x => x.TreatmentTypeId)
            .NotEmpty().WithMessage("TreatmentTypeId is required.");

        RuleFor(x => x.StartTime)
            .NotEmpty().WithMessage("StartTime is required.");

        RuleFor(x => x.EndTime)
            .NotEmpty().WithMessage("EndTime is required.")
            .GreaterThan(x => x.StartTime).WithMessage("EndTime must be after StartTime.");

        RuleFor(x => x.PatientName)
            .MaximumLength(255).WithMessage("PatientName must not exceed 255 characters.");

        RuleFor(x => x.PatientPhone)
            .MaximumLength(50).WithMessage("PatientPhone must not exceed 50 characters.");

        RuleFor(x => x.Color)
            .MaximumLength(7).WithMessage("Color must not exceed 7 characters.")
            .Matches(@"^#[0-9A-Fa-f]{6}$").When(x => x.Color is not null)
            .WithMessage("Color must be a valid hex color (e.g., #FF5733).");
    }
}
