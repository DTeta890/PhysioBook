using PhysioBook.Domain.Common;

namespace PhysioBook.Domain.Entities;

public class Appointment : BaseEntity
{
    public Guid TherapistId { get; set; }
    public Guid? PatientId { get; set; }
    public Guid TreatmentTypeId { get; set; }
    public string? PatientName { get; set; }
    public string? PatientPhone { get; set; }
    public DateTimeOffset StartTime { get; set; }
    public DateTimeOffset EndTime { get; set; }
    public string Status { get; set; } = "scheduled";
    public string? Notes { get; set; }
    public string? CancellationReason { get; set; }
    public bool IsWalkIn { get; set; }
    public string? Color { get; set; }

    // Navigation
    public User Therapist { get; set; } = null!;
    public TreatmentType TreatmentType { get; set; } = null!;
    public Tenant Tenant { get; set; } = null!;
}
