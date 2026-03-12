using MediatR;
using PhysioBook.Application.Common.Exceptions;
using PhysioBook.Application.Common.Interfaces;
using PhysioBook.Application.PatientDocuments.DTOs;
using PhysioBook.Domain.Entities;

namespace PhysioBook.Application.PatientDocuments.Queries;

public sealed record GetDocumentDownloadQuery(Guid Id) : IRequest<DocumentDownloadDto>;

public sealed class GetDocumentDownloadQueryHandler : IRequestHandler<GetDocumentDownloadQuery, DocumentDownloadDto>
{
    private readonly IPatientDocumentRepository _repository;
    private readonly IDocumentStorageService _storageService;

    private const string BucketName = "physiobook-documents";

    public GetDocumentDownloadQueryHandler(
        IPatientDocumentRepository repository,
        IDocumentStorageService storageService)
    {
        _repository = repository;
        _storageService = storageService;
    }

    public async Task<DocumentDownloadDto> Handle(GetDocumentDownloadQuery request, CancellationToken cancellationToken)
    {
        var document = await _repository.GetByIdAsync(request.Id, cancellationToken);

        if (document is null)
        {
            throw new NotFoundException(nameof(PatientDocument), request.Id);
        }

        var downloadUrl = await _storageService.GetPresignedUrlAsync(
            BucketName,
            document.StorageKey,
            3600,
            cancellationToken);

        return new DocumentDownloadDto(
            document.Id,
            document.FileName,
            document.ContentType,
            downloadUrl);
    }
}
