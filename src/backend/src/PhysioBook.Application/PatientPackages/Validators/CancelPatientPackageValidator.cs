using FluentValidation;
using PhysioBook.Application.PatientPackages.Commands;

namespace PhysioBook.Application.PatientPackages.Validators;

public sealed class CancelPatientPackageValidator : AbstractValidator<CancelPatientPackageCommand>
{
    public CancelPatientPackageValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Patient package id is required.");
    }
}
