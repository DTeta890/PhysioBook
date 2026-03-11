namespace PhysioBook.Application.RecurringRules.DTOs;

public sealed record RecurringRuleDto(
    Guid Id,
    Guid TherapistId,
    string TherapistName,
    Guid? PatientId,
    string? PatientName,
    string? PatientPhone,
    Guid TreatmentTypeId,
    string TreatmentTypeName,
    string Frequency,
    int DayOfWeek,
    string StartTimeOfDay,
    string EndTimeOfDay,
    DateTimeOffset StartsFrom,
    DateTimeOffset? EndsAt,
    int? MaxOccurrences,
    string? Notes,
    string? Color,
    bool IsActive,
    DateTimeOffset CreatedAt);
