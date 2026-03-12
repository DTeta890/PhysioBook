using PhysioBook.Domain.Common;

namespace PhysioBook.Domain.Entities;

public class PatientPackage : BaseEntity
{
    public Guid PatientId { get; set; }
    public Guid TreatmentPackageId { get; set; }
    public int SessionsUsed { get; set; }
    public int SessionsRemaining => TreatmentPackage?.TotalSessions - SessionsUsed ?? 0;
    public DateTimeOffset PurchasedAt { get; set; }
    public DateTimeOffset? ExpiresAt { get; set; }
    public string Status { get; set; } = "active";
    public string? Notes { get; set; }

    // Navigation
    public TreatmentPackage TreatmentPackage { get; set; } = null!;
    public Tenant Tenant { get; set; } = null!;
}
