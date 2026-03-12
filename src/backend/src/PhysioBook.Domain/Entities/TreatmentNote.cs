using PhysioBook.Domain.Common;

namespace PhysioBook.Domain.Entities;

public class TreatmentNote : BaseEntity
{
    public Guid AppointmentId { get; set; }
    public Guid? PatientId { get; set; }
    public Guid TherapistId { get; set; }

    // SOAP
    public string? Subjective { get; set; }
    public string? Objective { get; set; }
    public string? Assessment { get; set; }
    public string? Plan { get; set; }

    // Additional
    public string? Diagnosis { get; set; }
    public string? TreatmentProvided { get; set; }
    public int? PainLevelBefore { get; set; }
    public int? PainLevelAfter { get; set; }
    public string? RangeOfMotionNotes { get; set; }
    public string? ExercisesPrescribed { get; set; }
    public string? FollowUpInstructions { get; set; }
    public bool IsSigned { get; set; }
    public DateTimeOffset? SignedAt { get; set; }

    // Navigation
    public Appointment Appointment { get; set; } = null!;
    public Patient? Patient { get; set; }
    public User Therapist { get; set; } = null!;
    public Tenant Tenant { get; set; } = null!;
}
