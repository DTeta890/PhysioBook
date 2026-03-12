using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PhysioBook.Application.Common;
using PhysioBook.Application.WalkIns.Commands;
using PhysioBook.Application.WalkIns.DTOs;
using PhysioBook.Application.WalkIns.Queries;

namespace PhysioBook.Api.Controllers;

[Authorize]
public sealed class WalkInsController : BaseApiController
{
    [HttpPost]
    public async Task<ActionResult<ApiResponse<WalkInEntryDto>>> CheckIn(
        [FromBody] CheckInWalkInRequest request)
    {
        var command = new CheckInWalkInCommand(
            request.PatientId,
            request.PatientName,
            request.PatientPhone,
            request.TreatmentTypeId,
            request.ReasonForVisit,
            request.Priority,
            request.Notes);

        var result = await Mediator.Send(command);
        return CreatedResponse(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<WalkInEntryDto>>>> GetQueue(
        [FromQuery] string? status = null,
        [FromQuery] DateTimeOffset? date = null)
    {
        var result = await Mediator.Send(new GetWalkInQueueQuery(status, date));
        return OkResponse(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponse<WalkInEntryDto>>> GetById(Guid id)
    {
        var result = await Mediator.Send(new GetWalkInEntryQuery(id));
        return OkResponse(result);
    }

    [HttpGet("stats")]
    public async Task<ActionResult<ApiResponse<WalkInStatsDto>>> GetStats(
        [FromQuery] DateTimeOffset from,
        [FromQuery] DateTimeOffset to)
    {
        var result = await Mediator.Send(new GetWalkInStatsQuery(from, to));
        return OkResponse(result);
    }

    [HttpPatch("{id:guid}/call")]
    public async Task<ActionResult<ApiResponse<WalkInEntryDto>>> Call(
        Guid id,
        [FromBody] CallWalkInRequest request)
    {
        var result = await Mediator.Send(new CallWalkInCommand(id, request.TherapistId));
        return OkResponse(result);
    }

    [HttpPatch("{id:guid}/complete")]
    public async Task<ActionResult<ApiResponse<WalkInEntryDto>>> Complete(Guid id)
    {
        var result = await Mediator.Send(new CompleteWalkInCommand(id));
        return OkResponse(result);
    }

    [HttpPatch("{id:guid}/cancel")]
    public async Task<ActionResult<ApiResponse<WalkInEntryDto>>> Cancel(
        Guid id,
        [FromBody] CancelWalkInRequest? request)
    {
        var result = await Mediator.Send(new CancelWalkInCommand(id, request?.Reason));
        return OkResponse(result);
    }

    [HttpPost("{id:guid}/convert")]
    public async Task<ActionResult<ApiResponse<WalkInEntryDto>>> Convert(
        Guid id,
        [FromBody] ConvertToAppointmentRequest request)
    {
        var command = new ConvertToAppointmentCommand(
            id,
            request.TherapistId,
            request.TreatmentTypeId,
            request.StartTime,
            request.EndTime,
            request.Notes);

        var result = await Mediator.Send(command);
        return CreatedResponse(nameof(GetById), new { id = result.Id }, result);
    }
}

// Request DTOs
public sealed record CheckInWalkInRequest(
    Guid? PatientId,
    string PatientName,
    string? PatientPhone,
    Guid? TreatmentTypeId,
    string? ReasonForVisit,
    int? Priority,
    string? Notes);

public sealed record CallWalkInRequest(Guid TherapistId);

public sealed record CancelWalkInRequest(string? Reason);

public sealed record ConvertToAppointmentRequest(
    Guid TherapistId,
    Guid TreatmentTypeId,
    DateTimeOffset StartTime,
    DateTimeOffset EndTime,
    string? Notes);
