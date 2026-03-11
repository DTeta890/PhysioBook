namespace PhysioBook.Application.Therapists.DTOs;

public sealed record TherapistDto(
    Guid Id,
    string Email,
    string FirstName,
    string LastName,
    string? Phone,
    string? Specialization,
    string? Color,
    string? AvatarUrl,
    bool IsActive,
    DateTimeOffset CreatedAt);
