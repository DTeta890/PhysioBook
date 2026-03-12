namespace PhysioBook.Application.PatientPackages.DTOs;

public sealed record PatientPackageDto(
    Guid Id,
    Guid PatientId,
    Guid TreatmentPackageId,
    string TreatmentPackageName,
    string TreatmentTypeName,
    int TotalSessions,
    int SessionsUsed,
    int SessionsRemaining,
    decimal Price,
    DateTimeOffset PurchasedAt,
    DateTimeOffset? ExpiresAt,
    string Status,
    string? Notes,
    DateTimeOffset CreatedAt);
