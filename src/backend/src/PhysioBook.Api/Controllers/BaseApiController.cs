using MediatR;
using Microsoft.AspNetCore.Mvc;
using PhysioBook.Application.Common;

namespace PhysioBook.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public abstract class BaseApiController : ControllerBase
{
    private ISender? _mediator;

    protected ISender Mediator =>
        _mediator ??= HttpContext.RequestServices.GetRequiredService<ISender>();

    protected ActionResult<ApiResponse<T>> OkResponse<T>(T data, Dictionary<string, object>? meta = null)
    {
        return Ok(ApiResponse<T>.Success(data, meta));
    }

    protected ActionResult<ApiResponse<T>> CreatedResponse<T>(string actionName, object routeValues, T data)
    {
        return CreatedAtAction(actionName, routeValues, ApiResponse<T>.Success(data));
    }
}
