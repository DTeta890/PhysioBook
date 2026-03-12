using MediatR;
using PhysioBook.Application.Common.Interfaces;
using PhysioBook.Application.PatientDocuments.DTOs;
using PhysioBook.Domain.Entities;

namespace PhysioBook.Application.PatientDocuments.Commands;

public sealed record UploadDocumentCommand(
    Guid PatientId,
    string FileName,
    string ContentType,
    long FileSizeBytes,
    string Category,
    string? Description,
    Stream FileStream) : IRequest<PatientDocumentDto>;

public sealed class UploadDocumentCommandHandler : IRequestHandler<UploadDocumentCommand, PatientDocumentDto>
{
    private readonly IPatientDocumentRepository _repository;
    private readonly IDocumentStorageService _storageService;
    private readonly ICurrentTenantService _tenantService;
    private readonly ICurrentUserService _userService;
    private const string BucketName = "physiobook-documents";

    public UploadDocumentCommandHandler(
        IPatientDocumentRepository repository,
        IDocumentStorageService storageService,
        ICurrentTenantService tenantService,
        ICurrentUserService userService)
    {
        _repository = repository;
        _storageService = storageService;
        _tenantService = tenantService;
        _userService = userService;
    }

    public async Task<PatientDocumentDto> Handle(UploadDocumentCommand request, CancellationToken cancellationToken)
    {
        var tenantId = _tenantService.TenantId;
        var storageKey = $"{tenantId}/{request.PatientId}/{Guid.NewGuid()}_{request.FileName}";

        await _storageService.UploadAsync(
            BucketName,
            storageKey,
            request.FileStream,
            request.ContentType,
            cancellationToken);

        // CreatedAt, UpdatedAt, TenantId, and Id are set automatically by ApplicationDbContext.SaveChangesAsync
        var document = new PatientDocument
        {
            PatientId = request.PatientId,
            FileName = request.FileName,
            StorageKey = storageKey,
            ContentType = request.ContentType,
            FileSizeBytes = request.FileSizeBytes,
            Category = request.Category.ToLowerInvariant(),
            Description = request.Description,
            UploadedBy = _userService.UserId.ToString(),
        };

        await _repository.AddAsync(document, cancellationToken);

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
