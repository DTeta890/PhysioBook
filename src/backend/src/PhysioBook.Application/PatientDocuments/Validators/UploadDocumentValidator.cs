using FluentValidation;
using PhysioBook.Application.PatientDocuments.Commands;
using PhysioBook.Domain.Enums;

namespace PhysioBook.Application.PatientDocuments.Validators;

public sealed class UploadDocumentValidator : AbstractValidator<UploadDocumentCommand>
{
    private const long MaxFileSizeBytes = 50L * 1024 * 1024; // 50 MB

    public UploadDocumentValidator()
    {
        RuleFor(x => x.PatientId)
            .NotEmpty().WithMessage("PatientId is required.");

        RuleFor(x => x.FileName)
            .NotEmpty().WithMessage("FileName is required.")
            .MaximumLength(500).WithMessage("FileName must not exceed 500 characters.");

        RuleFor(x => x.ContentType)
            .NotEmpty().WithMessage("ContentType is required.");

        RuleFor(x => x.FileSizeBytes)
            .GreaterThan(0).WithMessage("FileSizeBytes must be greater than 0.")
            .LessThanOrEqualTo(MaxFileSizeBytes).WithMessage($"FileSizeBytes must not exceed {MaxFileSizeBytes} bytes (50 MB).");

        RuleFor(x => x.Category)
            .NotEmpty().WithMessage("Category is required.")
            .Must(c => DocumentCategory.IsValid(c)).WithMessage("Category must be one of: xray, mri, referral, consent, insurance, other.");
    }
}
