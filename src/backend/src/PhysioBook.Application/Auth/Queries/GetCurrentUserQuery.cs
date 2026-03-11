using MediatR;
using Microsoft.EntityFrameworkCore;
using PhysioBook.Application.Auth.DTOs;
using PhysioBook.Application.Common.Exceptions;
using PhysioBook.Application.Common.Interfaces;

namespace PhysioBook.Application.Auth.Queries;

public record GetCurrentUserQuery : IRequest<UserDto>;

public class GetCurrentUserQueryHandler : IRequestHandler<GetCurrentUserQuery, UserDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public GetCurrentUserQueryHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<UserDto> Handle(GetCurrentUserQuery request, CancellationToken cancellationToken)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == _currentUserService.UserId, cancellationToken)
            ?? throw new NotFoundException("User", _currentUserService.UserId);

        return new UserDto(user.Id, user.Email, user.FirstName, user.LastName,
            user.Role, user.IsTherapist, user.Specialization, user.Phone,
            user.AvatarUrl, user.TenantId);
    }
}
