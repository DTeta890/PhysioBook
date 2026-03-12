using MediatR;
using PhysioBook.Application.Common.Interfaces;
using PhysioBook.Application.PatientDocuments.DTOs;

namespace PhysioBook.Application.PatientDocuments.Queries;

public sealed record GetPatientDocumentsQuery(
    Guid PatientId,
    string? Category = null,
    int Page = 1,
    int PageSize = 20) : IRequest<PatientDocumentsResult>;

public sealed record PatientDocumentsResult(
    List<PatientDocumentDto> Items,
    int TotalCount,
    int Page,
    int PageSize);

public sealed class GetPatientDocumentsQueryHandler : IRequestHandler<GetPatientDocumentsQuery, PatientDocumentsResult>
{
    private readonly IPatientDocumentRepository _repository;

    public GetPatientDocumentsQueryHandler(IPatientDocumentRepository repository)
    {
        _repository = repository;
    }

    public async Task<PatientDocumentsResult> Handle(GetPatientDocumentsQuery request, CancellationToken cancellationToken)
    {
        var (items, totalCount) = await _repository.GetByPatientIdAsync(
            request.PatientId,
            request.Category,
            request.Page,
            request.PageSize,
            cancellationToken);

        var dtos = items.Select(d => new PatientDocumentDto(
            d.Id,
            d.PatientId,
            d.FileName,
            d.ContentType,
            d.FileSizeBytes,
            d.Category,
            d.Description,
            d.UploadedBy,
            d.CreatedAt)).ToList();

        return new PatientDocumentsResult(dtos, totalCount, request.Page, request.PageSize);
    }
}
