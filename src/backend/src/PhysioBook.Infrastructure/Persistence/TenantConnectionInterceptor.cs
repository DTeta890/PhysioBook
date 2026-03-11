using System.Data.Common;
using Microsoft.EntityFrameworkCore.Diagnostics;
using PhysioBook.Application.Common.Interfaces;

namespace PhysioBook.Infrastructure.Persistence;

/// <summary>
/// Interceptor that sets the app.current_tenant PostgreSQL session variable
/// when a connection is opened, ensuring RLS policies apply to all queries.
/// </summary>
public class TenantConnectionInterceptor : DbConnectionInterceptor
{
    private readonly ICurrentTenantService _tenantService;

    public TenantConnectionInterceptor(ICurrentTenantService tenantService)
    {
        _tenantService = tenantService;
    }

    public override async Task ConnectionOpenedAsync(
        DbConnection connection,
        ConnectionEndEventData eventData,
        CancellationToken cancellationToken = default)
    {
        if (_tenantService.TenantId != Guid.Empty)
        {
            await using var cmd = connection.CreateCommand();
            cmd.CommandText = $"SET app.current_tenant = '{_tenantService.TenantId}'";
            await cmd.ExecuteNonQueryAsync(cancellationToken);
        }
    }

    public override void ConnectionOpened(
        DbConnection connection,
        ConnectionEndEventData eventData)
    {
        if (_tenantService.TenantId != Guid.Empty)
        {
            using var cmd = connection.CreateCommand();
            cmd.CommandText = $"SET app.current_tenant = '{_tenantService.TenantId}'";
            cmd.ExecuteNonQuery();
        }
    }
}
