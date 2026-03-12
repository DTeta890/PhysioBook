namespace PhysioBook.Application.TreatmentPackages.DTOs;

public sealed record TreatmentPackageDto(
    Guid Id,
    string Name,
    Guid TreatmentTypeId,
    string TreatmentTypeName,
    int TotalSessions,
    decimal Price,
    int? ValidityDays,
    string? Description,
    bool IsActive,
    DateTimeOffset CreatedAt);
