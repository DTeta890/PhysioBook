using PhysioBook.Domain.Common;

namespace PhysioBook.Domain.Entities;

public class RecurringRule : BaseEntity
{
    public Guid TherapistId { get; set; }
    public Guid? PatientId { get; set; }
    public Guid TreatmentTypeId { get; set; }
    public string? PatientName { get; set; }
    public string? PatientPhone { get; set; }
    public string Frequency { get; set; } = "weekly"; // weekly, biweekly, monthly
    public int DayOfWeek { get; set; } // 0=Sunday, 1=Monday, ..., 6=Saturday
    public TimeOnly StartTimeOfDay { get; set; }
    public TimeOnly EndTimeOfDay { get; set; }
    public DateTimeOffset StartsFrom { get; set; } // rule effective from
    public DateTimeOffset? EndsAt { get; set; } // null = indefinite
    public int? MaxOccurrences { get; set; } // null = unlimited
    public string? Notes { get; set; }
    public string? Color { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation
    public User Therapist { get; set; } = null!;
    public TreatmentType TreatmentType { get; set; } = null!;
    public Tenant Tenant { get; set; } = null!;
    public ICollection<Appointment> Appointments { get; set; } = [];
}
