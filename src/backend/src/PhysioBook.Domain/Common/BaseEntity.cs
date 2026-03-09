namespace PhysioBook.Domain.Common;

public abstract class BaseEntity
{
    public Guid Id { get; set; }

    public Guid TenantId { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset UpdatedAt { get; set; }
}
