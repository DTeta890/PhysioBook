namespace PhysioBook.Application.TreatmentTypes.DTOs;

public sealed record TreatmentTypeDto(
    Guid Id,
    string Name,
    string? Description,
    int DurationMinutes,
    decimal Price,
    string? Color,
    bool IsActive,
    DateTimeOffset CreatedAt);
