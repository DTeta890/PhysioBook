using PhysioBook.Application.Common.Interfaces;
using PhysioBook.Domain.Common;
using PhysioBook.Infrastructure.Persistence;
using PhysioBook.Infrastructure.Services;

namespace PhysioBook.UnitTests.Multitenancy;

public class EntityFilterExpressionTests
{
    private class TestEntity : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
    }

    [Fact]
    public void CreateTenantFilter_ReturnsValidExpression()
    {
        var tenantService = new CurrentTenantService();
        var tenantId = Guid.NewGuid();
        tenantService.SetTenant(tenantId);

        var filter = EntityFilterExpression.CreateTenantFilter(typeof(TestEntity), tenantService);

        Assert.NotNull(filter);
        Assert.Single(filter.Parameters);
        Assert.Equal(typeof(bool), filter.ReturnType);
    }

    [Fact]
    public void CreateTenantFilter_FiltersCorrectly()
    {
        var tenantService = new CurrentTenantService();
        var tenantId = Guid.NewGuid();
        tenantService.SetTenant(tenantId);

        var filter = EntityFilterExpression.CreateTenantFilter(typeof(TestEntity), tenantService);
        var compiled = filter.Compile();

        var matchingEntity = new TestEntity { TenantId = tenantId, Name = "Match" };
        var nonMatchingEntity = new TestEntity { TenantId = Guid.NewGuid(), Name = "NoMatch" };

        Assert.True((bool)compiled.DynamicInvoke(matchingEntity)!);
        Assert.False((bool)compiled.DynamicInvoke(nonMatchingEntity)!);
    }
}
