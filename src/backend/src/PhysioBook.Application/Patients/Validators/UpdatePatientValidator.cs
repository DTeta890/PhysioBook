using FluentValidation;
using PhysioBook.Application.Patients.Commands;

namespace PhysioBook.Application.Patients.Validators;

public sealed class UpdatePatientValidator : AbstractValidator<UpdatePatientCommand>
{
    private static readonly string[] ValidGenders = ["male", "female", "other"];

    public UpdatePatientValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Id is required.");

        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("FirstName is required.")
            .MaximumLength(100).WithMessage("FirstName must not exceed 100 characters.");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("LastName is required.")
            .MaximumLength(100).WithMessage("LastName must not exceed 100 characters.");

        RuleFor(x => x.Email)
            .EmailAddress().When(x => !string.IsNullOrEmpty(x.Email))
            .WithMessage("Email must be a valid email address.")
            .MaximumLength(255).WithMessage("Email must not exceed 255 characters.");

        RuleFor(x => x.Phone)
            .MaximumLength(50).WithMessage("Phone must not exceed 50 characters.");

        RuleFor(x => x.Gender)
            .Must(g => g is null || ValidGenders.Contains(g))
            .WithMessage("Gender must be 'male', 'female', or 'other'.");

        RuleFor(x => x.City)
            .MaximumLength(100).WithMessage("City must not exceed 100 characters.");

        RuleFor(x => x.EmergencyContactName)
            .MaximumLength(200).WithMessage("EmergencyContactName must not exceed 200 characters.");

        RuleFor(x => x.EmergencyContactPhone)
            .MaximumLength(50).WithMessage("EmergencyContactPhone must not exceed 50 characters.");
    }
}
