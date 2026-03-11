using FluentValidation;
using PhysioBook.Application.Appointments.Commands;

namespace PhysioBook.Application.Appointments.Validators;

public sealed class UpdateAppointmentStatusValidator : AbstractValidator<UpdateAppointmentStatusCommand>
{
    private static readonly string[] ValidStatuses =
        ["scheduled", "confirmed", "in_progress", "completed", "cancelled", "no_show"];

    public UpdateAppointmentStatusValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Appointment Id is required.");

        RuleFor(x => x.Status)
            .NotEmpty().WithMessage("Status is required.")
            .Must(s => ValidStatuses.Contains(s))
            .WithMessage("Status must be one of: scheduled, confirmed, in_progress, completed, cancelled, no_show.");

        RuleFor(x => x.CancellationReason)
            .NotEmpty().When(x => x.Status == "cancelled")
            .WithMessage("CancellationReason is required when cancelling an appointment.");
    }
}
