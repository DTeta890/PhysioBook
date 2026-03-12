using MediatR;
using PhysioBook.Application.Common.Exceptions;
using PhysioBook.Application.Common.Interfaces;
using PhysioBook.Application.PatientDocuments.DTOs;
using PhysioBook.Domain.Entities;

namespace PhysioBook.Application.PatientDocuments.Queries;

public sealed record GetDocumentQuery(Guid Id) : IRequest<PatientDocumentDto>;

public sealed class GetDocumentQueryHandler : IRequestHandler<GetDocumentQuery, PatientDocumentDto>
{
    private readonly IPatientDocumentRepository _repository;

    public GetDocumentQueryHandler(IPatientDocumentRepository repository)
    {
        _repository = repository;
    }

    public async Task<PatientDocumentDto> Handle(GetDocumentQuery request, CancellationToken cancellationToken)
    {
        var document = await _repository.GetByIdAsync(request.Id, cancellationToken);

        if (document is null)
        {
            throw new NotFoundException(nameof(PatientDocument), request.Id);
        }

        return new PatientDocumentDto(
            document.Id,
            document.PatientId,
            document.FileName,
            document.ContentType,
            document.FileSizeBytes,
            document.Category,
            document.Description,
            document.UploadedBy,
            document.CreatedAt);
    }
}
