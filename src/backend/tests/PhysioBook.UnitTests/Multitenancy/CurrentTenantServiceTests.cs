using PhysioBook.Infrastructure.Services;

namespace PhysioBook.UnitTests.Multitenancy;

public class CurrentTenantServiceTests
{
    [Fact]
    public void SetTenant_ShouldUpdateTenantId()
    {
        var service = new CurrentTenantService();
        var tenantId = Guid.NewGuid();

        service.SetTenant(tenantId, "Test Clinic");

        Assert.Equal(tenantId, service.TenantId);
        Assert.Equal("Test Clinic", service.TenantName);
    }

    [Fact]
    public void TenantId_DefaultsToEmpty()
    {
        var service = new CurrentTenantService();

        Assert.Equal(Guid.Empty, service.TenantId);
        Assert.Null(service.TenantName);
    }

    [Fact]
    public void SetTenant_CanBeCalledMultipleTimes()
    {
        var service = new CurrentTenantService();
        var firstId = Guid.NewGuid();
        var secondId = Guid.NewGuid();

        service.SetTenant(firstId, "First");
        service.SetTenant(secondId, "Second");

        Assert.Equal(secondId, service.TenantId);
        Assert.Equal("Second", service.TenantName);
    }
}
