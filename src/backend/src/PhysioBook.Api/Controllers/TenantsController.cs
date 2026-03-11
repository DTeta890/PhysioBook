using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PhysioBook.Application.Common;
using PhysioBook.Application.Common.Interfaces;
using PhysioBook.Application.Tenants.Commands;
using PhysioBook.Application.Tenants.DTOs;
using PhysioBook.Application.Tenants.Queries;

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

    [HttpPost]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<TenantDto>>> Create(
        [FromBody] CreateTenantRequest request)
    {
        var command = new CreateTenantCommand(
            request.Name,
            request.Slug,
            request.ContactEmail,
            request.Phone,
            request.Address,
            request.City,
            request.Subdomain,
            request.AdminEmail,
            request.AdminPassword,
            request.AdminFirstName,
            request.AdminLastName);

        var result = await Mediator.Send(command);
        return CreatedResponse(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<TenantDto>>> GetById(Guid id)
    {
        var result = await Mediator.Send(new GetTenantQuery(id));
        return OkResponse(result);
    }

    [HttpPut("{id:guid}")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<TenantDto>>> Update(
        Guid id,
        [FromBody] UpdateTenantRequest request)
    {
        var command = new UpdateTenantCommand(
            id,
            request.Name,
            request.ContactEmail,
            request.Phone,
            request.Address,
            request.City,
            request.Subdomain,
            request.IsActive);

        var result = await Mediator.Send(command);
        return OkResponse(result);
    }

    [HttpGet("all")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<List<TenantDto>>>> GetAll()
    {
        var result = await Mediator.Send(new GetTenantsQuery());
        return OkResponse(result);
    }
}

// Request DTOs
public sealed record CreateTenantRequest(
    string Name,
    string Slug,
    string ContactEmail,
    string? Phone,
    string? Address,
    string? City,
    string? Subdomain,
    string AdminEmail,
    string AdminPassword,
    string AdminFirstName,
    string AdminLastName);

public sealed record UpdateTenantRequest(
    string Name,
    string ContactEmail,
    string? Phone,
    string? Address,
    string? City,
    string? Subdomain,
    bool IsActive);
