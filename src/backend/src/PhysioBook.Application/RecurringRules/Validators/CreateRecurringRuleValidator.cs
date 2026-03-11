using FluentValidation;
using PhysioBook.Application.RecurringRules.Commands;

namespace PhysioBook.Application.RecurringRules.Validators;

public sealed class CreateRecurringRuleValidator : AbstractValidator<CreateRecurringRuleCommand>
{
    private static readonly string[] ValidFrequencies = ["weekly", "biweekly", "monthly"];

    public CreateRecurringRuleValidator()
    {
        RuleFor(x => x.TherapistId)
            .NotEmpty().WithMessage("Therapist ID is required.");

        RuleFor(x => x.TreatmentTypeId)
            .NotEmpty().WithMessage("Treatment type ID is required.");

        RuleFor(x => x.Frequency)
            .NotEmpty().WithMessage("Frequency is required.")
            .Must(f => ValidFrequencies.Contains(f))
            .WithMessage("Frequency must be one of: weekly, biweekly, monthly.");

        RuleFor(x => x.DayOfWeek)
            .InclusiveBetween(0, 6).WithMessage("Day of week must be between 0 (Sunday) and 6 (Saturday).");

        RuleFor(x => x.StartTimeOfDay)
            .NotEmpty().WithMessage("Start time is required.")
            .Must(BeValidTimeFormat).WithMessage("Start time must be in HH:mm format.");

        RuleFor(x => x.EndTimeOfDay)
            .NotEmpty().WithMessage("End time is required.")
            .Must(BeValidTimeFormat).WithMessage("End time must be in HH:mm format.");

        RuleFor(x => x)
            .Must(x => BeEndTimeAfterStartTime(x.StartTimeOfDay, x.EndTimeOfDay))
            .When(x => BeValidTimeFormat(x.StartTimeOfDay) && BeValidTimeFormat(x.EndTimeOfDay))
            .WithMessage("End time must be after start time.");

        RuleFor(x => x.StartsFrom)
            .NotEmpty().WithMessage("Start date is required.");

        RuleFor(x => x.EndsAt)
            .GreaterThan(x => x.StartsFrom)
            .When(x => x.EndsAt.HasValue)
            .WithMessage("End date must be after start date.");

        RuleFor(x => x.MaxOccurrences)
            .GreaterThan(0)
            .When(x => x.MaxOccurrences.HasValue)
            .WithMessage("Max occurrences must be greater than 0.");

        RuleFor(x => x.PatientName)
            .MaximumLength(255).WithMessage("Patient name must not exceed 255 characters.");

        RuleFor(x => x.PatientPhone)
            .MaximumLength(50).WithMessage("Patient phone must not exceed 50 characters.");

        RuleFor(x => x.Color)
            .MaximumLength(7).WithMessage("Color must not exceed 7 characters.")
            .Matches(@"^#[0-9A-Fa-f]{6}$").When(x => x.Color is not null)
            .WithMessage("Color must be a valid hex color (e.g., #FF5733).");
    }

    private static bool BeValidTimeFormat(string? time)
    {
        if (string.IsNullOrEmpty(time)) return false;
        return TimeOnly.TryParse(time, out _);
    }

    private static bool BeEndTimeAfterStartTime(string startTime, string endTime)
    {
        if (!TimeOnly.TryParse(startTime, out var start)) return false;
        if (!TimeOnly.TryParse(endTime, out var end)) return false;
        return end > start;
    }
}
