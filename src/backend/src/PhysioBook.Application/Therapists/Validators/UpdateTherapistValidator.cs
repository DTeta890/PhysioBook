using FluentValidation;
using PhysioBook.Application.Therapists.Commands;

namespace PhysioBook.Application.Therapists.Validators;

public sealed class UpdateTherapistValidator : AbstractValidator<UpdateTherapistCommand>
{
    public UpdateTherapistValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Therapist ID is required.");

        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required.")
            .MaximumLength(100).WithMessage("First name must not exceed 100 characters.");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name is required.")
            .MaximumLength(100).WithMessage("Last name must not exceed 100 characters.");

        RuleFor(x => x.Phone)
            .MaximumLength(20).WithMessage("Phone must not exceed 20 characters.")
            .When(x => x.Phone is not null);

        RuleFor(x => x.Specialization)
            .MaximumLength(200).WithMessage("Specialization must not exceed 200 characters.")
            .When(x => x.Specialization is not null);

        RuleFor(x => x.Color)
            .Matches("^#[0-9A-Fa-f]{6}$").WithMessage("Color must be a valid hex color (e.g., #FF5733).")
            .When(x => x.Color is not null);
    }
}
