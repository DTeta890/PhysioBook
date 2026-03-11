using FluentValidation;
using PhysioBook.Application.Therapists.Commands;

namespace PhysioBook.Application.Therapists.Validators;

public sealed class CreateTherapistValidator : AbstractValidator<CreateTherapistCommand>
{
    public CreateTherapistValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email format.");

        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required.")
            .MaximumLength(100).WithMessage("First name must not exceed 100 characters.");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name is required.")
            .MaximumLength(100).WithMessage("Last name must not exceed 100 characters.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters.")
            .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
            .Matches("[a-z]").WithMessage("Password must contain at least one lowercase letter.")
            .Matches("[0-9]").WithMessage("Password must contain at least one number.");

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
