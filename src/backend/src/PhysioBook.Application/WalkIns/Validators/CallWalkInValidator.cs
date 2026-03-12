using FluentValidation;
using PhysioBook.Application.WalkIns.Commands;

namespace PhysioBook.Application.WalkIns.Validators;

public sealed class CallWalkInValidator : AbstractValidator<CallWalkInCommand>
{
    public CallWalkInValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Id is required.");

        RuleFor(x => x.TherapistId)
            .NotEmpty().WithMessage("TherapistId is required.");
    }
}
