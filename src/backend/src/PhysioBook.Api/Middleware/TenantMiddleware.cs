using System.Security.Claims;
using PhysioBook.Application.Common.Interfaces;

namespace PhysioBook.Api.Middleware;

public class TenantMiddleware
{
    private readonly RequestDelegate _next;

    public TenantMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, ICurrentTenantService tenantService)
    {
        // Try to get tenant from JWT claim first, then from header
        var tenantId = context.User?.FindFirstValue("tenant_id")
                       ?? context.Request.Headers["X-Tenant-Id"].FirstOrDefault();

        if (Guid.TryParse(tenantId, out var parsedTenantId))
        {
            tenantService.SetTenant(parsedTenantId);
        }

        await _next(context);
    }
}
