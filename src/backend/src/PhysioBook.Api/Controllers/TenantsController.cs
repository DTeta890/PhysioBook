using Microsoft.AspNetCore.Mvc;
using PhysioBook.Application.Common;
using PhysioBook.Application.Common.Interfaces;

namespace PhysioBook.Api.Controllers;

public class TenantsController : BaseApiController
{
    private readonly ICurrentTenantService _tenantService;

    public TenantsController(ICurrentTenantService tenantService)
    {
        _tenantService = tenantService;
    }

    /// <summary>
    /// Returns the current tenant context (for debugging/verification).
    /// </summary>
    [HttpGet("current")]
    public ActionResult<ApiResponse<object>> GetCurrentTenant()
    {
        return OkResponse<object>(new
        {
            tenantId = _tenantService.TenantId,
            tenantName = _tenantService.TenantName,
        });
    }
}
