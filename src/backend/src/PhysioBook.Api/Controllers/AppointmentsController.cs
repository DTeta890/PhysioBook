using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PhysioBook.Application.Appointments.Commands;
using PhysioBook.Application.Appointments.DTOs;
using PhysioBook.Application.Appointments.Queries;
using PhysioBook.Application.Common;



namespace PhysioBook.Api.Controllers;

[Authorize]
public class AppointmentsController : BaseApiController
{
    [HttpPost]
    public async Task<ActionResult<ApiResponse<AppointmentDto>>> Create(
        [FromBody] CreateAppointmentRequest request)
    {
        var command = new CreateAppointmentCommand(
            request.TherapistId,
            request.PatientId,
            request.PatientName,
            request.PatientPhone,
            request.TreatmentTypeId,
            request.StartTime,
            request.EndTime,
            request.Notes,
            request.IsWalkIn,
            request.Color);

        var result = await Mediator.Send(command);
        return CreatedResponse(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<AppointmentDto>>>> GetAll(
        [FromQuery] DateTimeOffset startDate,
        [FromQuery] DateTimeOffset endDate,
        [FromQuery] Guid? therapistId = null)
    {
        var result = await Mediator.Send(new GetAppointmentsQuery(startDate, endDate, therapistId));
        return OkResponse(result);
    }

    [HttpGet("conflicts")]
    public async Task<ActionResult<ApiResponse<ConflictCheckResult>>> CheckConflicts(
        [FromQuery] Guid therapistId,
        [FromQuery] DateTimeOffset startTime,
        [FromQuery] DateTimeOffset endTime,
        [FromQuery] Guid? excludeAppointmentId = null)
    {
        var result = await Mediator.Send(new CheckConflictsQuery(therapistId, startTime, endTime, excludeAppointmentId));
        return OkResponse(result);
    }

    [HttpGet("availability")]
    public async Task<ActionResult<ApiResponse<TherapistAvailabilityResult>>> GetAvailability(
        [FromQuery] Guid therapistId,
        [FromQuery] DateOnly date)
    {
        var result = await Mediator.Send(new GetTherapistAvailabilityQuery(therapistId, date));
        return OkResponse(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponse<AppointmentDto>>> GetById(Guid id)
    {
        var result = await Mediator.Send(new GetAppointmentQuery(id));
        return OkResponse(result);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ApiResponse<AppointmentDto>>> Update(
        Guid id,
        [FromBody] UpdateAppointmentRequest request)
    {
        var command = new UpdateAppointmentCommand(
            id,
            request.TherapistId,
            request.PatientId,
            request.PatientName,
            request.PatientPhone,
            request.TreatmentTypeId,
            request.StartTime,
            request.EndTime,
            request.Notes,
            request.Color);

        var result = await Mediator.Send(command);
        return OkResponse(result);
    }

    [HttpPatch("{id:guid}/status")]
    public async Task<ActionResult<ApiResponse<AppointmentDto>>> UpdateStatus(
        Guid id,
        [FromBody] UpdateAppointmentStatusRequest request)
    {
        var command = new UpdateAppointmentStatusCommand(
            id,
            request.Status,
            request.CancellationReason);

        var result = await Mediator.Send(command);
        return OkResponse(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> Delete(Guid id)
    {
        await Mediator.Send(new DeleteAppointmentCommand(id));
        return NoContent();
    }
}

// Request DTOs
public sealed record CreateAppointmentRequest(
    Guid TherapistId,
    Guid? PatientId,
    string? PatientName,
    string? PatientPhone,
    Guid TreatmentTypeId,
    DateTimeOffset StartTime,
    DateTimeOffset EndTime,
    string? Notes,
    bool IsWalkIn,
    string? Color);

public sealed record UpdateAppointmentRequest(
    Guid TherapistId,
    Guid? PatientId,
    string? PatientName,
    string? PatientPhone,
    Guid TreatmentTypeId,
    DateTimeOffset StartTime,
    DateTimeOffset EndTime,
    string? Notes,
    string? Color);

public sealed record UpdateAppointmentStatusRequest(
    string Status,
    string? CancellationReason);
