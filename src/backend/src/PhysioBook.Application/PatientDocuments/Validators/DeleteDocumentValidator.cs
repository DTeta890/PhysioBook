using FluentValidation;
using PhysioBook.Application.PatientDocuments.Commands;

namespace PhysioBook.Application.PatientDocuments.Validators;

public sealed class DeleteDocumentValidator : AbstractValidator<DeleteDocumentCommand>
{
    public DeleteDocumentValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Id is required.");
    }
}
