namespace PhysioBook.Application.PatientDocuments.DTOs;

public sealed record PatientDocumentDto(
    Guid Id,
    Guid PatientId,
    string FileName,
    string ContentType,
    long FileSizeBytes,
    string Category,
    string? Description,
    string UploadedBy,
    DateTimeOffset CreatedAt);

public sealed record DocumentDownloadDto(
    Guid Id,
    string FileName,
    string ContentType,
    string DownloadUrl);
