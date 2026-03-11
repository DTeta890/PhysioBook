using MediatR;
using Microsoft.EntityFrameworkCore;
using PhysioBook.Application.Common.Interfaces;

namespace PhysioBook.Application.Auth.Commands;

public record LogoutCommand(string RefreshToken) : IRequest;

public class LogoutCommandHandler : IRequestHandler<LogoutCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly IPasswordService _passwordService;
    private readonly IDateTimeProvider _dateTimeProvider;

    public LogoutCommandHandler(
        IApplicationDbContext context,
        IPasswordService passwordService,
        IDateTimeProvider dateTimeProvider)
    {
        _context = context;
        _passwordService = passwordService;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        var tokens = await _context.RefreshTokens
            .IgnoreQueryFilters()
            .Where(r => r.RevokedAt == null)
            .ToListAsync(cancellationToken);

        var token = tokens.FirstOrDefault(t => _passwordService.Verify(request.RefreshToken, t.TokenHash));

        if (token is not null)
        {
            token.RevokedAt = _dateTimeProvider.Now;
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
