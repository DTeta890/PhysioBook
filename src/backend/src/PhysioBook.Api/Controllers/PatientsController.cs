using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PhysioBook.Application.Common;
using PhysioBook.Application.Patients.Commands;
using PhysioBook.Application.Patients.DTOs;
using PhysioBook.Application.Patients.Queries;

namespace PhysioBook.Api.Controllers;

[Authorize]
public class PatientsController : BaseApiController
{
    [HttpPost]
    public async Task<ActionResult<ApiResponse<PatientDto>>> Create(
        [FromBody] CreatePatientRequest request)
    {
        var command = new CreatePatientCommand(
            request.FirstName,
            request.LastName,
            request.Email,
            request.Phone,
            request.DateOfBirth,
            request.Gender,
            request.Address,
            request.City,
            request.EmergencyContactName,
            request.EmergencyContactPhone,
            request.MedicalHistory,
            request.Allergies,
            request.Notes);

        var result = await Mediator.Send(command);
        return CreatedResponse(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<PatientDto>>>> GetAll(
        [FromQuery] string? search = null,
        [FromQuery] bool? isActive = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var result = await Mediator.Send(new GetPatientsQuery(search, isActive, page, pageSize));
        return OkResponse(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponse<PatientDto>>> GetById(Guid id)
    {
        var result = await Mediator.Send(new GetPatientQuery(id));
        return OkResponse(result);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ApiResponse<PatientDto>>> Update(
        Guid id,
        [FromBody] UpdatePatientRequest request)
    {
        var command = new UpdatePatientCommand(
            id,
            request.FirstName,
            request.LastName,
            request.Email,
            request.Phone,
            request.DateOfBirth,
            request.Gender,
            request.Address,
            request.City,
            request.EmergencyContactName,
            request.EmergencyContactPhone,
            request.MedicalHistory,
            request.Allergies,
            request.Notes,
            request.IsActive);

        var result = await Mediator.Send(command);
        return OkResponse(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> Delete(Guid id)
    {
        await Mediator.Send(new DeletePatientCommand(id));
        return NoContent();
    }
}

// Request DTOs
public sealed record CreatePatientRequest(
    string FirstName,
    string LastName,
    string? Email,
    string? Phone,
    DateOnly? DateOfBirth,
    string? Gender,
    string? Address,
    string? City,
    string? EmergencyContactName,
    string? EmergencyContactPhone,
    string? MedicalHistory,
    string? Allergies,
    string? Notes);

public sealed record UpdatePatientRequest(
    string FirstName,
    string LastName,
    string? Email,
    string? Phone,
    DateOnly? DateOfBirth,
    string? Gender,
    string? Address,
    string? City,
    string? EmergencyContactName,
    string? EmergencyContactPhone,
    string? MedicalHistory,
    string? Allergies,
    string? Notes,
    bool IsActive);
