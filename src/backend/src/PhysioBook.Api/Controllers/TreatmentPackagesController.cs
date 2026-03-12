using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PhysioBook.Application.Common;
using PhysioBook.Application.TreatmentPackages.Commands;
using PhysioBook.Application.TreatmentPackages.DTOs;
using PhysioBook.Application.TreatmentPackages.Queries;

namespace PhysioBook.Api.Controllers;

[Authorize]
public class TreatmentPackagesController : BaseApiController
{
    [HttpPost]
    public async Task<ActionResult<ApiResponse<TreatmentPackageDto>>> Create(
        [FromBody] CreateTreatmentPackageRequest request)
    {
        var command = new CreateTreatmentPackageCommand(
            request.Name,
            request.TreatmentTypeId,
            request.TotalSessions,
            request.Price,
            request.ValidityDays,
            request.Description);

        var result = await Mediator.Send(command);
        return CreatedResponse(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<TreatmentPackageDto>>>> GetAll(
        [FromQuery] Guid? treatmentTypeId = null,
        [FromQuery] bool? isActive = null)
    {
        var result = await Mediator.Send(new GetTreatmentPackagesQuery(treatmentTypeId, isActive));
        return OkResponse(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponse<TreatmentPackageDto>>> GetById(Guid id)
    {
        var result = await Mediator.Send(new GetTreatmentPackageQuery(id));
        return OkResponse(result);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ApiResponse<TreatmentPackageDto>>> Update(
        Guid id,
        [FromBody] UpdateTreatmentPackageRequest request)
    {
        var command = new UpdateTreatmentPackageCommand(
            id,
            request.Name,
            request.TreatmentTypeId,
            request.TotalSessions,
            request.Price,
            request.ValidityDays,
            request.Description,
            request.IsActive);

        var result = await Mediator.Send(command);
        return OkResponse(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> Delete(Guid id)
    {
        await Mediator.Send(new DeleteTreatmentPackageCommand(id));
        return NoContent();
    }
}

// Request DTOs
public sealed record CreateTreatmentPackageRequest(
    string Name,
    Guid TreatmentTypeId,
    int TotalSessions,
    decimal Price,
    int? ValidityDays,
    string? Description);

public sealed record UpdateTreatmentPackageRequest(
    string Name,
    Guid TreatmentTypeId,
    int TotalSessions,
    decimal Price,
    int? ValidityDays,
    string? Description,
    bool IsActive);
