using FluentValidation;
using PhysioBook.Application.TreatmentTypes.Commands;

namespace PhysioBook.Application.TreatmentTypes.Validators;

public sealed class UpdateTreatmentTypeValidator : AbstractValidator<UpdateTreatmentTypeCommand>
{
    public UpdateTreatmentTypeValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Treatment type ID is required.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Treatment type name is required.")
            .MaximumLength(255).WithMessage("Treatment type name must not exceed 255 characters.");

        RuleFor(x => x.DurationMinutes)
            .GreaterThan(0).WithMessage("Duration must be greater than 0 minutes.")
            .LessThanOrEqualTo(480).WithMessage("Duration must not exceed 480 minutes (8 hours).");

        RuleFor(x => x.Price)
            .GreaterThanOrEqualTo(0).WithMessage("Price must be zero or positive.");

        RuleFor(x => x.Color)
            .MaximumLength(7).WithMessage("Color must not exceed 7 characters.")
            .Matches(@"^#[0-9A-Fa-f]{6}$").When(x => x.Color is not null)
            .WithMessage("Color must be a valid hex color (e.g., #FF5733).");
    }
}
