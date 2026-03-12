using MediatR;
using PhysioBook.Application.Common.Exceptions;
using PhysioBook.Application.Common.Interfaces;
using PhysioBook.Domain.Entities;

namespace PhysioBook.Application.PatientDocuments.Commands;

public sealed record DeleteDocumentCommand(Guid Id) : IRequest<Unit>;

public sealed class DeleteDocumentCommandHandler : IRequestHandler<DeleteDocumentCommand, Unit>
{
    private readonly IPatientDocumentRepository _repository;
    private readonly IDocumentStorageService _storageService;

    private const string BucketName = "physiobook-documents";

    public DeleteDocumentCommandHandler(
        IPatientDocumentRepository repository,
        IDocumentStorageService storageService)
    {
        _repository = repository;
        _storageService = storageService;
    }

    public async Task<Unit> Handle(DeleteDocumentCommand request, CancellationToken cancellationToken)
    {
        var document = await _repository.GetByIdAsync(request.Id, cancellationToken);

        if (document is null)
        {
            throw new NotFoundException(nameof(PatientDocument), request.Id);
        }

        await _storageService.DeleteAsync(BucketName, document.StorageKey, cancellationToken);
        await _repository.RemoveAsync(document, cancellationToken);

        return Unit.Value;
    }
}
