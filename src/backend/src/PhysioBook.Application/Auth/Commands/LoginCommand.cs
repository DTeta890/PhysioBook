using MediatR;
using Microsoft.EntityFrameworkCore;
using PhysioBook.Application.Auth.DTOs;
using PhysioBook.Application.Common;
using PhysioBook.Application.Common.Exceptions;
using PhysioBook.Application.Common.Interfaces;

namespace PhysioBook.Application.Auth.Commands;

public record LoginCommand(string Email, string Password, Guid TenantId) : IRequest<AuthResponse>;

public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthResponse>
{
    private readonly IApplicationDbContext _context;
    private readonly IJwtService _jwtService;
    private readonly IPasswordService _passwordService;
    private readonly IDateTimeProvider _dateTimeProvider;

    public LoginCommandHandler(
        IApplicationDbContext context,
        IJwtService jwtService,
        IPasswordService passwordService,
        IDateTimeProvider dateTimeProvider)
    {
        _context = context;
        _jwtService = jwtService;
        _passwordService = passwordService;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<AuthResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        // Find user by email within tenant (bypass global filter, manual tenant check)
        var user = await _context.Users
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(
                u => u.Email == request.Email && u.TenantId == request.TenantId,
                cancellationToken);

        if (user is null || !_passwordService.Verify(request.Password, user.PasswordHash))
        {
            throw new UnauthorizedException("Invalid email or password.");
        }

        if (!user.IsActive)
        {
            throw new UnauthorizedException("Account is deactivated. Contact your administrator.");
        }

        // Update last login
        user.LastLoginAt = _dateTimeProvider.Now;

        // Generate tokens
        var accessToken = _jwtService.GenerateAccessToken(user);
        var refreshTokenValue = _jwtService.GenerateRefreshToken();
        var refreshTokenHash = _passwordService.Hash(refreshTokenValue);

        var refreshToken = new Domain.Entities.RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            TokenHash = refreshTokenHash,
            ExpiresAt = _dateTimeProvider.Now.AddDays(7),
            CreatedAt = _dateTimeProvider.Now,
        };

        _context.RefreshTokens.Add(refreshToken);
        await _context.SaveChangesAsync(cancellationToken);

        var expiresAt = _dateTimeProvider.Now.AddMinutes(15);

        return new AuthResponse(
            accessToken,
            refreshTokenValue,
            expiresAt,
            MapToDto(user));
    }

    private static UserDto MapToDto(Domain.Entities.User user) => new(
        user.Id,
        user.Email,
        user.FirstName,
        user.LastName,
        user.Role,
        user.IsTherapist,
        user.Specialization,
        user.Phone,
        user.AvatarUrl,
        user.TenantId);
}
