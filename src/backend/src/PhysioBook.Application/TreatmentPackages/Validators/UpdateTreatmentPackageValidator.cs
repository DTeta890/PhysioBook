using FluentValidation;
using PhysioBook.Application.TreatmentPackages.Commands;

namespace PhysioBook.Application.TreatmentPackages.Validators;

public sealed class UpdateTreatmentPackageValidator : AbstractValidator<UpdateTreatmentPackageCommand>
{
    public UpdateTreatmentPackageValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Package id is required.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Package name is required.")
            .MaximumLength(200).WithMessage("Package name must not exceed 200 characters.");

        RuleFor(x => x.TreatmentTypeId)
            .NotEmpty().WithMessage("Treatment type is required.");

        RuleFor(x => x.TotalSessions)
            .GreaterThan(0).WithMessage("Total sessions must be greater than 0.");

        RuleFor(x => x.Price)
            .GreaterThanOrEqualTo(0).WithMessage("Price must be zero or positive.");

        RuleFor(x => x.ValidityDays)
            .GreaterThan(0).WithMessage("Validity days must be greater than 0.")
            .When(x => x.ValidityDays.HasValue);

        RuleFor(x => x.Description)
            .MaximumLength(1000).WithMessage("Description must not exceed 1000 characters.");
    }
}
