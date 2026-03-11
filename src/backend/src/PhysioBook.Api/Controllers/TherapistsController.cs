using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PhysioBook.Application.Common;
using PhysioBook.Application.Therapists.Commands;
using PhysioBook.Application.Therapists.DTOs;
using PhysioBook.Application.Therapists.Queries;

namespace PhysioBook.Api.Controllers;

[Authorize]
public class TherapistsController : BaseApiController
{
    [HttpPost]
    public async Task<ActionResult<ApiResponse<TherapistDto>>> Create(
        [FromBody] CreateTherapistRequest request)
    {
        var command = new CreateTherapistCommand(
            request.Email,
            request.FirstName,
            request.LastName,
            request.Password,
            request.Phone,
            request.Specialization,
            request.Color);

        var result = await Mediator.Send(command);
        return CreatedResponse(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<TherapistDto>>>> GetAll(
        [FromQuery] bool? isActive = null)
    {
        var result = await Mediator.Send(new GetTherapistsQuery(isActive));
        return OkResponse(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponse<TherapistDto>>> GetById(Guid id)
    {
        var result = await Mediator.Send(new GetTherapistQuery(id));
        return OkResponse(result);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ApiResponse<TherapistDto>>> Update(
        Guid id,
        [FromBody] UpdateTherapistRequest request)
    {
        var command = new UpdateTherapistCommand(
            id,
            request.FirstName,
            request.LastName,
            request.Phone,
            request.Specialization,
            request.Color,
            request.IsActive);

        var result = await Mediator.Send(command);
        return OkResponse(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> Delete(Guid id)
    {
        await Mediator.Send(new DeleteTherapistCommand(id));
        return NoContent();
    }
}

// Request DTOs
public sealed record CreateTherapistRequest(
    string Email,
    string FirstName,
    string LastName,
    string Password,
    string? Phone,
    string? Specialization,
    string? Color);

public sealed record UpdateTherapistRequest(
    string FirstName,
    string LastName,
    string? Phone,
    string? Specialization,
    string? Color,
    bool IsActive);
