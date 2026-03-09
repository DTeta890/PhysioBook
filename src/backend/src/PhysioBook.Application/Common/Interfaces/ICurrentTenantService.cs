namespace PhysioBook.Application.Common.Interfaces;

public interface ICurrentTenantService
{
    Guid TenantId { get; }
    string? TenantName { get; }
    void SetTenant(Guid tenantId, string? tenantName = null);
}
