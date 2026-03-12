using FluentValidation;
using PhysioBook.Application.PatientPackages.Commands;

namespace PhysioBook.Application.PatientPackages.Validators;

public sealed class PurchasePackageValidator : AbstractValidator<PurchasePackageCommand>
{
    public PurchasePackageValidator()
    {
        RuleFor(x => x.PatientId)
            .NotEmpty().WithMessage("Patient is required.");

        RuleFor(x => x.TreatmentPackageId)
            .NotEmpty().WithMessage("Treatment package is required.");

        RuleFor(x => x.Notes)
            .MaximumLength(1000).WithMessage("Notes must not exceed 1000 characters.");
    }
}
