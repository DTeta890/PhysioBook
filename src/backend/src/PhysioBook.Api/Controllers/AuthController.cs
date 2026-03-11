using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PhysioBook.Application.Auth.Commands;
using PhysioBook.Application.Auth.DTOs;
using PhysioBook.Application.Auth.Queries;
using PhysioBook.Application.Common;

namespace PhysioBook.Api.Controllers;

public class AuthController : BaseApiController
{
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<AuthResponse>>> Login(
        [FromBody] LoginRequest request,
        [FromHeader(Name = "X-Tenant-Id")] Guid tenantId)
    {
        var command = new LoginCommand(request.Email, request.Password, tenantId);
        var result = await Mediator.Send(command);
        return OkResponse(result);
    }

    [HttpPost("register")]
    [Authorize(Roles = "owner,admin")]
    public async Task<ActionResult<ApiResponse<UserDto>>> Register(
        [FromBody] RegisterRequest request)
    {
        var command = new RegisterCommand(
            request.Email,
            request.Password,
            request.FirstName,
            request.LastName,
            request.Phone,
            request.Role,
            request.IsTherapist,
            request.Specialization,
            request.Color);

        var result = await Mediator.Send(command);
        return CreatedResponse(nameof(GetMe), null!, result);
    }

    [HttpPost("refresh")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<AuthResponse>>> Refresh(
        [FromBody] RefreshRequest request)
    {
        var command = new RefreshTokenCommand(request.RefreshToken);
        var result = await Mediator.Send(command);
        return OkResponse(result);
    }

    [HttpPost("logout")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<object>>> Logout(
        [FromBody] RefreshRequest request)
    {
        await Mediator.Send(new LogoutCommand(request.RefreshToken));
        return OkResponse<object>(new { message = "Logged out successfully." });
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<UserDto>>> GetMe()
    {
        var result = await Mediator.Send(new GetCurrentUserQuery());
        return OkResponse(result);
    }

    [HttpPut("change-password")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<object>>> ChangePassword(
        [FromBody] ChangePasswordRequest request)
    {
        await Mediator.Send(new ChangePasswordCommand(request.CurrentPassword, request.NewPassword));
        return OkResponse<object>(new { message = "Password changed successfully." });
    }
}

// Request DTOs (lightweight, separate from MediatR commands)
public record LoginRequest(string Email, string Password);
public record RegisterRequest(
    string Email,
    string Password,
    string FirstName,
    string LastName,
    string? Phone,
    string Role,
    bool IsTherapist,
    string? Specialization,
    string? Color);
public record RefreshRequest(string RefreshToken);
public record ChangePasswordRequest(string CurrentPassword, string NewPassword);
