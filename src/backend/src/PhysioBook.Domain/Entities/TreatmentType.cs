using PhysioBook.Domain.Common;

namespace PhysioBook.Domain.Entities;

public class TreatmentType : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int DurationMinutes { get; set; } = 30;
    public decimal Price { get; set; }
    public string? Color { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation
    public Tenant Tenant { get; set; } = null!;
}
