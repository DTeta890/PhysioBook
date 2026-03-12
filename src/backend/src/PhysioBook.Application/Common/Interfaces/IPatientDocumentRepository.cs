using PhysioBook.Domain.Entities;

namespace PhysioBook.Application.Common.Interfaces;

public interface IPatientDocumentRepository
{
    Task<PatientDocument?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<(List<PatientDocument> Items, int TotalCount)> GetByPatientIdAsync(Guid patientId, string? category, int page, int pageSize, CancellationToken cancellationToken = default);
    Task AddAsync(PatientDocument document, CancellationToken cancellationToken = default);
    Task RemoveAsync(PatientDocument document, CancellationToken cancellationToken = default);
}
