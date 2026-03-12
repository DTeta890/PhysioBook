using PhysioBook.Domain.Common;

namespace PhysioBook.Domain.Entities;

public class TreatmentPackage : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public Guid TreatmentTypeId { get; set; }
    public int TotalSessions { get; set; }
    public decimal Price { get; set; }
    public int? ValidityDays { get; set; }
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation
    public TreatmentType TreatmentType { get; set; } = null!;
    public Tenant Tenant { get; set; } = null!;
}
