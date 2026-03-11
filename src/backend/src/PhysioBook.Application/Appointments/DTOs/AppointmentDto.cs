namespace PhysioBook.Application.Appointments.DTOs;

public sealed record AppointmentDto(
    Guid Id,
    Guid TherapistId,
    string TherapistName,
    Guid? PatientId,
    string? PatientName,
    string? PatientPhone,
    Guid TreatmentTypeId,
    string TreatmentTypeName,
    DateTimeOffset StartTime,
    DateTimeOffset EndTime,
    string Status,
    string? Notes,
    string? CancellationReason,
    bool IsWalkIn,
    string? Color,
    DateTimeOffset CreatedAt);
