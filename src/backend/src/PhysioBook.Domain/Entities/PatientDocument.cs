using PhysioBook.Domain.Common;

namespace PhysioBook.Domain.Entities;

public class PatientDocument : BaseEntity
{
    public Guid PatientId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string StorageKey { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long FileSizeBytes { get; set; }
    public string Category { get; set; } = "other";
    public string? Description { get; set; }
    public string UploadedBy { get; set; } = string.Empty;

    // Navigation
    public Tenant Tenant { get; set; } = null!;
}
