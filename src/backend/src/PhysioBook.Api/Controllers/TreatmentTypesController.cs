using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PhysioBook.Application.Common;
using PhysioBook.Application.TreatmentTypes.Commands;
using PhysioBook.Application.TreatmentTypes.DTOs;
using PhysioBook.Application.TreatmentTypes.Queries;

namespace PhysioBook.Api.Controllers;

[Authorize]
public class TreatmentTypesController : BaseApiController
{
    [HttpPost]
    public async Task<ActionResult<ApiResponse<TreatmentTypeDto>>> Create(
        [FromBody] CreateTreatmentTypeRequest request)
    {
        var command = new CreateTreatmentTypeCommand(
            request.Name,
            request.Description,
            request.DurationMinutes,
            request.Price,
            request.Color);

        var result = await Mediator.Send(command);
        return CreatedResponse(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<TreatmentTypeDto>>>> GetAll(
        [FromQuery] bool? isActive = null)
    {
        var result = await Mediator.Send(new GetTreatmentTypesQuery(isActive));
        return OkResponse(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponse<TreatmentTypeDto>>> GetById(Guid id)
    {
        var result = await Mediator.Send(new GetTreatmentTypeQuery(id));
        return OkResponse(result);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ApiResponse<TreatmentTypeDto>>> Update(
        Guid id,
        [FromBody] UpdateTreatmentTypeRequest request)
    {
        var command = new UpdateTreatmentTypeCommand(
            id,
            request.Name,
            request.Description,
            request.DurationMinutes,
            request.Price,
            request.Color,
            request.IsActive);

        var result = await Mediator.Send(command);
        return OkResponse(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> Delete(Guid id)
    {
        await Mediator.Send(new DeleteTreatmentTypeCommand(id));
        return NoContent();
    }
}

// Request DTOs
public sealed record CreateTreatmentTypeRequest(
    string Name,
    string? Description,
    int DurationMinutes,
    decimal Price,
    string? Color);

public sealed record UpdateTreatmentTypeRequest(
    string Name,
    string? Description,
    int DurationMinutes,
    decimal Price,
    string? Color,
    bool IsActive);
