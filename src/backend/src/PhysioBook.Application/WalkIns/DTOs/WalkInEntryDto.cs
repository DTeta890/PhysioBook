namespace PhysioBook.Application.WalkIns.DTOs;

public sealed record WalkInEntryDto(
    Guid Id,
    Guid? PatientId,
    string PatientName,
    string? PatientPhone,
    Guid? TreatmentTypeId,
    string? TreatmentTypeName,
    string? ReasonForVisit,
    int Priority,
    string Status,
    DateTimeOffset CheckedInAt,
    DateTimeOffset? CalledAt,
    DateTimeOffset? CompletedAt,
    Guid? AssignedTherapistId,
    string? AssignedTherapistName,
    Guid? ConvertedAppointmentId,
    int QueuePosition,
    double WaitTimeMinutes,
    string? Notes,
    DateTimeOffset CreatedAt);
