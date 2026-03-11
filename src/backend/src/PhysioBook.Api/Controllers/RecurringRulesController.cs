using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PhysioBook.Application.Appointments.DTOs;
using PhysioBook.Application.Common;
using PhysioBook.Application.RecurringRules.Commands;
using PhysioBook.Application.RecurringRules.DTOs;
using PhysioBook.Application.RecurringRules.Queries;

namespace PhysioBook.Api.Controllers;

[Authorize]
public class RecurringRulesController : BaseApiController
{
    [HttpPost]
    public async Task<ActionResult<ApiResponse<RecurringRuleDto>>> Create(
        [FromBody] CreateRecurringRuleRequest request)
    {
        var command = new CreateRecurringRuleCommand(
            request.TherapistId,
            request.PatientId,
            request.PatientName,
            request.PatientPhone,
            request.TreatmentTypeId,
            request.Frequency,
            request.DayOfWeek,
            request.StartTimeOfDay,
            request.EndTimeOfDay,
            request.StartsFrom,
            request.EndsAt,
            request.MaxOccurrences,
            request.Notes,
            request.Color);

        var result = await Mediator.Send(command);
        return CreatedResponse(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<RecurringRuleDto>>>> GetAll(
        [FromQuery] Guid? therapistId = null)
    {
        var result = await Mediator.Send(new GetRecurringRulesQuery(therapistId));
        return OkResponse(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponse<RecurringRuleDto>>> GetById(Guid id)
    {
        var result = await Mediator.Send(new GetRecurringRuleQuery(id));
        return OkResponse(result);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ApiResponse<RecurringRuleDto>>> Update(
        Guid id,
        [FromBody] UpdateRecurringRuleRequest request)
    {
        var command = new UpdateRecurringRuleCommand(
            id,
            request.TherapistId,
            request.PatientId,
            request.PatientName,
            request.PatientPhone,
            request.TreatmentTypeId,
            request.Frequency,
            request.DayOfWeek,
            request.StartTimeOfDay,
            request.EndTimeOfDay,
            request.StartsFrom,
            request.EndsAt,
            request.MaxOccurrences,
            request.Notes,
            request.Color,
            request.IsActive);

        var result = await Mediator.Send(command);
        return OkResponse(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> Delete(Guid id)
    {
        await Mediator.Send(new DeleteRecurringRuleCommand(id));
        return NoContent();
    }

    [HttpPost("{id:guid}/generate")]
    public async Task<ActionResult<ApiResponse<List<AppointmentDto>>>> Generate(
        Guid id,
        [FromBody] GenerateAppointmentsRequest request)
    {
        var command = new GenerateAppointmentsCommand(id, request.FromDate, request.ToDate);
        var result = await Mediator.Send(command);
        return OkResponse(result);
    }
}

// Request DTOs
public sealed record CreateRecurringRuleRequest(
    Guid TherapistId,
    Guid? PatientId,
    string? PatientName,
    string? PatientPhone,
    Guid TreatmentTypeId,
    string Frequency,
    int DayOfWeek,
    string StartTimeOfDay,
    string EndTimeOfDay,
    DateTimeOffset StartsFrom,
    DateTimeOffset? EndsAt,
    int? MaxOccurrences,
    string? Notes,
    string? Color);

public sealed record UpdateRecurringRuleRequest(
    Guid TherapistId,
    Guid? PatientId,
    string? PatientName,
    string? PatientPhone,
    Guid TreatmentTypeId,
    string Frequency,
    int DayOfWeek,
    string StartTimeOfDay,
    string EndTimeOfDay,
    DateTimeOffset StartsFrom,
    DateTimeOffset? EndsAt,
    int? MaxOccurrences,
    string? Notes,
    string? Color,
    bool IsActive);

public sealed record GenerateAppointmentsRequest(
    DateTimeOffset FromDate,
    DateTimeOffset ToDate);
