using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using PhysioBook.Application.Common.Interfaces;
using PhysioBook.Infrastructure.Persistence;

namespace PhysioBook.Api.Middleware;

public class TenantMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<TenantMiddleware> _logger;

    // Paths that don't require a tenant
    private static readonly string[] TenantFreeEndpoints =
    [
        "/health",
        "/swagger",
        "/api/v1/auth/login",
        "/api/v1/auth/register",
        "/api/v1/booking",
        "/api/v1/tenants",
    ];

    public TenantMiddleware(RequestDelegate next, ILogger<TenantMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(
        HttpContext context,
        ICurrentTenantService tenantService,
        ApplicationDbContext dbContext)
    {
        var path = context.Request.Path.Value ?? string.Empty;

        // Skip tenant resolution for tenant-free endpoints
        if (IsTenantFreeEndpoint(path))
        {
            await _next(context);
            return;
        }

        // 1. Try JWT claim
        var tenantIdStr = context.User?.FindFirstValue("tenant_id");

        // 2. Fallback to X-Tenant-Id header
        if (string.IsNullOrEmpty(tenantIdStr))
        {
            tenantIdStr = context.Request.Headers["X-Tenant-Id"].FirstOrDefault();
        }

        if (!Guid.TryParse(tenantIdStr, out var tenantId))
        {
            // No tenant found — if it's an API endpoint, reject
            if (path.StartsWith("/api/", StringComparison.OrdinalIgnoreCase))
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                await context.Response.WriteAsJsonAsync(new
                {
                    data = (object?)null,
                    errors = new[] { "Tenant identification required. Provide tenant_id in JWT or X-Tenant-Id header." },
                    meta = (object?)null,
                });
                return;
            }

            await _next(context);
            return;
        }

        // 3. Validate tenant exists in database (bypass query filter for this check)
        var tenantExists = await dbContext.Tenants
            .IgnoreQueryFilters()
            .AnyAsync(t => t.Id == tenantId);

        if (!tenantExists)
        {
            _logger.LogWarning("Tenant {TenantId} not found", tenantId);
            context.Response.StatusCode = StatusCodes.Status404NotFound;
            await context.Response.WriteAsJsonAsync(new
            {
                data = (object?)null,
                errors = new[] { "Tenant not found." },
                meta = (object?)null,
            });
            return;
        }

        // 4. Set tenant in scoped service
        tenantService.SetTenant(tenantId);

        // 5. Set PostgreSQL session variable for RLS
        await dbContext.Database.ExecuteSqlAsync(
            $"SET app.current_tenant = {tenantId.ToString()}");

        _logger.LogDebug("Tenant context set: {TenantId}", tenantId);

        await _next(context);
    }

    private static bool IsTenantFreeEndpoint(string path)
    {
        foreach (var endpoint in TenantFreeEndpoints)
        {
            if (path.StartsWith(endpoint, StringComparison.OrdinalIgnoreCase))
                return true;
        }
        return false;
    }
}
