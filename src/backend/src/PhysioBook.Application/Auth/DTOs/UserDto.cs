namespace PhysioBook.Application.Auth.DTOs;

public record UserDto(
    Guid Id,
    string Email,
    string FirstName,
    string LastName,
    string Role,
    bool IsTherapist,
    string? Specialization,
    string? Phone,
    string? AvatarUrl,
    Guid TenantId);
