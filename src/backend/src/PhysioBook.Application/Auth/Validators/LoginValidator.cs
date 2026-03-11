using FluentValidation;
using PhysioBook.Application.Auth.Commands;

namespace PhysioBook.Application.Auth.Validators;

public class LoginValidator : AbstractValidator<LoginCommand>
{
    public LoginValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email format.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.");

        RuleFor(x => x.TenantId)
            .NotEmpty().WithMessage("Tenant identification is required.");
    }
}
