using MediatR;
using Microsoft.EntityFrameworkCore;
using PhysioBook.Application.Auth.DTOs;
using PhysioBook.Application.Common.Exceptions;
using PhysioBook.Application.Common.Interfaces;

namespace PhysioBook.Application.Auth.Commands;

public record RefreshTokenCommand(string RefreshToken) : IRequest<AuthResponse>;

public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, AuthResponse>
{
    private readonly IApplicationDbContext _context;
    private readonly IJwtService _jwtService;
    private readonly IPasswordService _passwordService;
    private readonly IDateTimeProvider _dateTimeProvider;

    public RefreshTokenCommandHandler(
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

    public async Task<AuthResponse> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        // Find all non-revoked refresh tokens and check hash
        var storedTokens = await _context.RefreshTokens
            .IgnoreQueryFilters()
            .Include(r => r.User)
            .Where(r => r.RevokedAt == null)
            .ToListAsync(cancellationToken);

        var storedToken = storedTokens
            .FirstOrDefault(t => _passwordService.Verify(request.RefreshToken, t.TokenHash));

        if (storedToken is null)
        {
            throw new UnauthorizedException("Invalid refresh token.");
        }

        if (storedToken.IsExpired)
        {
            throw new UnauthorizedException("Refresh token has expired.");
        }

        if (!storedToken.User.IsActive)
        {
            throw new UnauthorizedException("Account is deactivated.");
        }

        // Revoke the old token (rotation)
        storedToken.RevokedAt = _dateTimeProvider.Now;

        // Generate new pair
        var accessToken = _jwtService.GenerateAccessToken(storedToken.User);
        var newRefreshTokenValue = _jwtService.GenerateRefreshToken();
        var newRefreshTokenHash = _passwordService.Hash(newRefreshTokenValue);

        var newRefreshToken = new Domain.Entities.RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = storedToken.UserId,
            TokenHash = newRefreshTokenHash,
            ExpiresAt = _dateTimeProvider.Now.AddDays(7),
            CreatedAt = _dateTimeProvider.Now,
        };

        _context.RefreshTokens.Add(newRefreshToken);
        await _context.SaveChangesAsync(cancellationToken);

        var expiresAt = _dateTimeProvider.Now.AddMinutes(15);
        var user = storedToken.User;

        return new AuthResponse(
            accessToken,
            newRefreshTokenValue,
            expiresAt,
            new UserDto(user.Id, user.Email, user.FirstName, user.LastName,
                user.Role, user.IsTherapist, user.Specialization, user.Phone,
                user.AvatarUrl, user.TenantId));
    }
}
