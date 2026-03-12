using PhysioBook.Domain.Common;

namespace PhysioBook.Domain.Entities;

public class WalkInEntry : BaseEntity
{
    public Guid? PatientId { get; set; }
    public string PatientName { get; set; } = string.Empty;
    public string? PatientPhone { get; set; }
    public Guid? TreatmentTypeId { get; set; }
    public string? ReasonForVisit { get; set; }
    public int Priority { get; set; } = 1;
    public string Status { get; set; } = "waiting";
    public DateTimeOffset CheckedInAt { get; set; }
    public DateTimeOffset? CalledAt { get; set; }
    public DateTimeOffset? CompletedAt { get; set; }
    public Guid? AssignedTherapistId { get; set; }
    public Guid? ConvertedAppointmentId { get; set; }
    public int QueuePosition { get; set; }
    public string? Notes { get; set; }

    // Navigation
    public TreatmentType? TreatmentType { get; set; }
    public User? AssignedTherapist { get; set; }
    public Appointment? ConvertedAppointment { get; set; }
    public Tenant Tenant { get; set; } = null!;
}
