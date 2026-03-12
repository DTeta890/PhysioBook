using FluentValidation;
using PhysioBook.Application.PatientPackages.Commands;

namespace PhysioBook.Application.PatientPackages.Validators;

public sealed class UseSessionValidator : AbstractValidator<UseSessionCommand>
{
    public UseSessionValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Patient package id is required.");
    }
}
