using System.Linq.Expressions;
using PhysioBook.Application.Common.Interfaces;
using PhysioBook.Domain.Common;

namespace PhysioBook.Infrastructure.Persistence;

public static class EntityFilterExpression
{
    public static LambdaExpression CreateTenantFilter(Type entityType, ICurrentTenantService tenantService)
    {
        var parameter = Expression.Parameter(entityType, "e");
        var tenantIdProperty = Expression.Property(parameter, nameof(BaseEntity.TenantId));
        var tenantIdValue = Expression.Property(
            Expression.Constant(tenantService),
            nameof(ICurrentTenantService.TenantId));
        var comparison = Expression.Equal(tenantIdProperty, tenantIdValue);
        return Expression.Lambda(comparison, parameter);
    }
}
