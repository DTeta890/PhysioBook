using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PhysioBook.Application.Common;
using PhysioBook.Application.PatientPackages.Commands;
using PhysioBook.Application.PatientPackages.DTOs;
using PhysioBook.Application.PatientPackages.Queries;

namespace PhysioBook.Api.Controllers;

[Authorize]
public class PatientPackagesController : BaseApiController
{
    [HttpPost]
    public async Task<ActionResult<ApiResponse<PatientPackageDto>>> Purchase(
        [FromBody] PurchasePackageRequest request)
    {
        var command = new PurchasePackageCommand(
            request.PatientId,
            request.TreatmentPackageId,
            request.Notes);

        var result = await Mediator.Send(command);
        return CreatedResponse(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpGet("by-patient/{patientId:guid}")]
    public async Task<ActionResult<ApiResponse<List<PatientPackageDto>>>> GetByPatient(
        Guid patientId,
        [FromQuery] string? status = null)
    {
        var result = await Mediator.Send(new GetPatientPackagesQuery(patientId, status));
        return OkResponse(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponse<PatientPackageDto>>> GetById(Guid id)
    {
        var result = await Mediator.Send(new GetPatientPackageQuery(id));
        return OkResponse(result);
    }

    [HttpPatch("{id:guid}/use-session")]
    public async Task<ActionResult<ApiResponse<PatientPackageDto>>> UseSession(Guid id)
    {
        var result = await Mediator.Send(new UseSessionCommand(id));
        return OkResponse(result);
    }

    [HttpPatch("{id:guid}/cancel")]
    public async Task<ActionResult<ApiResponse<PatientPackageDto>>> Cancel(Guid id)
    {
        var result = await Mediator.Send(new CancelPatientPackageCommand(id));
        return OkResponse(result);
    }
}

// Request DTOs
public sealed record PurchasePackageRequest(
    Guid PatientId,
    Guid TreatmentPackageId,
    string? Notes);
