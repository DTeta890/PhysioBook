using System.Net;
using System.Text.Json;
using PhysioBook.Application.Common;
using PhysioBook.Application.Common.Exceptions;
using ValidationException = PhysioBook.Application.Common.Exceptions.ValidationException;

namespace PhysioBook.Api.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var (statusCode, errors) = exception switch
        {
            ValidationException validationEx => (
                HttpStatusCode.BadRequest,
                validationEx.Errors
                    .SelectMany(e => e.Value.Select(v => $"{e.Key}: {v}"))
                    .ToList()),

            NotFoundException notFoundEx => (
                HttpStatusCode.NotFound,
                new List<string> { notFoundEx.Message }),

            ForbiddenException forbiddenEx => (
                HttpStatusCode.Forbidden,
                new List<string> { forbiddenEx.Message }),

            UnauthorizedException unauthorizedEx => (
                HttpStatusCode.Unauthorized,
                new List<string> { unauthorizedEx.Message }),

            ConflictException conflictEx => (
                HttpStatusCode.Conflict,
                new List<string> { conflictEx.Message }),

            _ => (
                HttpStatusCode.InternalServerError,
                new List<string> { "An unexpected error occurred." })
        };

        if (statusCode == HttpStatusCode.InternalServerError)
        {
            _logger.LogError(exception, "Unhandled exception");
        }

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        var response = ApiResponse<object>.Failure(errors);
        var json = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await context.Response.WriteAsync(json);
    }
}
