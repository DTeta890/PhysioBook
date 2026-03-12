using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PhysioBook.Application.Common;
using PhysioBook.Application.PatientDocuments.Commands;
using PhysioBook.Application.PatientDocuments.DTOs;
using PhysioBook.Application.PatientDocuments.Queries;

namespace PhysioBook.Api.Controllers;

[Authorize]
public class PatientDocumentsController : BaseApiController
{
    [HttpPost]
    [RequestSizeLimit(52_428_800)] // 50 MB
    public async Task<ActionResult<ApiResponse<PatientDocumentDto>>> Upload(
        [FromForm] UploadDocumentRequest request)
    {
        await using var stream = request.File.OpenReadStream();

        var command = new UploadDocumentCommand(
            request.PatientId,
            request.File.FileName,
            request.File.ContentType,
            request.File.Length,
            request.Category,
            request.Description,
            stream);

        var result = await Mediator.Send(command);
        return CreatedResponse(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpGet("by-patient/{patientId:guid}")]
    public async Task<ActionResult<ApiResponse<PatientDocumentsResult>>> GetByPatient(
        Guid patientId,
        [FromQuery] string? category = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var result = await Mediator.Send(new GetPatientDocumentsQuery(patientId, category, page, pageSize));
        return OkResponse(result, new Dictionary<string, object>
        {
            ["totalCount"] = result.TotalCount,
            ["page"] = result.Page,
            ["pageSize"] = result.PageSize
        });
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponse<PatientDocumentDto>>> GetById(Guid id)
    {
        var result = await Mediator.Send(new GetDocumentQuery(id));
        return OkResponse(result);
    }

    [HttpGet("{id:guid}/download")]
    public async Task<ActionResult<ApiResponse<DocumentDownloadDto>>> Download(Guid id)
    {
        var result = await Mediator.Send(new GetDocumentDownloadQuery(id));
        return OkResponse(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> Delete(Guid id)
    {
        await Mediator.Send(new DeleteDocumentCommand(id));
        return NoContent();
    }
}

// Request DTO for multipart form upload
public sealed record UploadDocumentRequest
{
    public Guid PatientId { get; init; }
    public string Category { get; init; } = "other";
    public string? Description { get; init; }
    public IFormFile File { get; init; } = null!;
}
