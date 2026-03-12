namespace PhysioBook.Application.Patients.DTOs;

public sealed record PatientDto(
    Guid Id,
    string FirstName,
    string LastName,
    string FullName,
    string? Email,
    string? Phone,
    DateOnly? DateOfBirth,
    string? Gender,
    string? Address,
    string? City,
    string? EmergencyContactName,
    string? EmergencyContactPhone,
    string? MedicalHistory,
    string? Allergies,
    string? Notes,
    bool IsActive,
    DateTimeOffset CreatedAt);
