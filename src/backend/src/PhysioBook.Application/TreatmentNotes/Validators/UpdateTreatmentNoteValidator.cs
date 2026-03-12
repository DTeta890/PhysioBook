using FluentValidation;
using PhysioBook.Application.TreatmentNotes.Commands;

namespace PhysioBook.Application.TreatmentNotes.Validators;

public sealed class UpdateTreatmentNoteValidator : AbstractValidator<UpdateTreatmentNoteCommand>
{
    public UpdateTreatmentNoteValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Id is required.");

        RuleFor(x => x.PainLevelBefore)
            .InclusiveBetween(0, 10).When(x => x.PainLevelBefore.HasValue)
            .WithMessage("PainLevelBefore must be between 0 and 10.");

        RuleFor(x => x.PainLevelAfter)
            .InclusiveBetween(0, 10).When(x => x.PainLevelAfter.HasValue)
            .WithMessage("PainLevelAfter must be between 0 and 10.");

        RuleFor(x => x)
            .Must(HaveAtLeastOneSoapField)
            .WithMessage("At least one SOAP field (Subjective, Objective, Assessment, or Plan) must be provided.")
            .WithName("SOAP");
    }

    private static bool HaveAtLeastOneSoapField(UpdateTreatmentNoteCommand command)
    {
        return !string.IsNullOrWhiteSpace(command.Subjective) ||
               !string.IsNullOrWhiteSpace(command.Objective) ||
               !string.IsNullOrWhiteSpace(command.Assessment) ||
               !string.IsNullOrWhiteSpace(command.Plan);
    }
}
