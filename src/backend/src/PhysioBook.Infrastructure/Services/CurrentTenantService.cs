using PhysioBook.Application.Common.Interfaces;

namespace PhysioBook.Infrastructure.Services;

public class CurrentTenantService : ICurrentTenantService
{
    public Guid TenantId { get; private set; }
    public string? TenantName { get; private set; }

    public void SetTenant(Guid tenantId, string? tenantName = null)
    {
        TenantId = tenantId;
        TenantName = tenantName;
    }
}
