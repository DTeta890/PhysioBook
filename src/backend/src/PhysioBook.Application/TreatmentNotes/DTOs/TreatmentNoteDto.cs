namespace PhysioBook.Application.TreatmentNotes.DTOs;

public sealed record TreatmentNoteDto(
    Guid Id,
    Guid AppointmentId,
    Guid? PatientId,
    string? PatientName,
    Guid TherapistId,
    string TherapistName,
    DateTimeOffset AppointmentDate,
    string? Subjective,
    string? Objective,
    string? Assessment,
    string? Plan,
    string? Diagnosis,
    string? TreatmentProvided,
    int? PainLevelBefore,
    int? PainLevelAfter,
    string? RangeOfMotionNotes,
    string? ExercisesPrescribed,
    string? FollowUpInstructions,
    bool IsSigned,
    DateTimeOffset? SignedAt,
    DateTimeOffset CreatedAt);
