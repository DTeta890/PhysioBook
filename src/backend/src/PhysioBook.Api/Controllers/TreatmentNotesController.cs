using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PhysioBook.Application.Common;
using PhysioBook.Application.TreatmentNotes.Commands;
using PhysioBook.Application.TreatmentNotes.DTOs;
using PhysioBook.Application.TreatmentNotes.Queries;

namespace PhysioBook.Api.Controllers;

[Authorize]
public class TreatmentNotesController : BaseApiController
{
    [HttpPost]
    public async Task<ActionResult<ApiResponse<TreatmentNoteDto>>> Create(
        [FromBody] CreateTreatmentNoteRequest request)
    {
        var command = new CreateTreatmentNoteCommand(
            request.AppointmentId,
            request.Subjective,
            request.Objective,
            request.Assessment,
            request.Plan,
            request.Diagnosis,
            request.TreatmentProvided,
            request.PainLevelBefore,
            request.PainLevelAfter,
            request.RangeOfMotionNotes,
            request.ExercisesPrescribed,
            request.FollowUpInstructions);

        var result = await Mediator.Send(command);
        return CreatedResponse(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponse<TreatmentNoteDto>>> GetById(Guid id)
    {
        var result = await Mediator.Send(new GetTreatmentNoteQuery(id));
        return OkResponse(result);
    }

    [HttpGet("by-appointment/{appointmentId:guid}")]
    public async Task<ActionResult<ApiResponse<TreatmentNoteDto>>> GetByAppointment(Guid appointmentId)
    {
        var result = await Mediator.Send(new GetTreatmentNoteByAppointmentQuery(appointmentId));
        return OkResponse(result);
    }

    [HttpGet("by-patient/{patientId:guid}")]
    public async Task<ActionResult<ApiResponse<PagedResult<TreatmentNoteDto>>>> GetByPatient(
        Guid patientId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var result = await Mediator.Send(new GetPatientTreatmentNotesQuery(patientId, page, pageSize));
        return OkResponse(result);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ApiResponse<TreatmentNoteDto>>> Update(
        Guid id,
        [FromBody] UpdateTreatmentNoteRequest request)
    {
        var command = new UpdateTreatmentNoteCommand(
            id,
            request.Subjective,
            request.Objective,
            request.Assessment,
            request.Plan,
            request.Diagnosis,
            request.TreatmentProvided,
            request.PainLevelBefore,
            request.PainLevelAfter,
            request.RangeOfMotionNotes,
            request.ExercisesPrescribed,
            request.FollowUpInstructions);

        var result = await Mediator.Send(command);
        return OkResponse(result);
    }

    [HttpPatch("{id:guid}/sign")]
    public async Task<ActionResult<ApiResponse<TreatmentNoteDto>>> Sign(Guid id)
    {
        var result = await Mediator.Send(new SignTreatmentNoteCommand(id));
        return OkResponse(result);
    }
}

// Request DTOs
public sealed record CreateTreatmentNoteRequest(
    Guid AppointmentId,
    string? Subjective,
    string? Objective,
    string? Assessment,
    string? Plan,
    string? Diagnosis,
    string? TreatmentProvided,
    int? PainLevelBefore,
    int? PainLevelAfter,
    string? RangeOfMotionNotes,
    string? ExercisesPrescribed,
    string? FollowUpInstructions);

public sealed record UpdateTreatmentNoteRequest(
    string? Subjective,
    string? Objective,
    string? Assessment,
    string? Plan,
    string? Diagnosis,
    string? TreatmentProvided,
    int? PainLevelBefore,
    int? PainLevelAfter,
    string? RangeOfMotionNotes,
    string? ExercisesPrescribed,
    string? FollowUpInstructions);
