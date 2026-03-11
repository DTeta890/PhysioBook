using MediatR;
using Microsoft.EntityFrameworkCore;
using PhysioBook.Application.Common.Exceptions;
using PhysioBook.Application.Common.Interfaces;
using PhysioBook.Application.Therapists.DTOs;
using PhysioBook.Domain.Entities;

namespace PhysioBook.Application.Therapists.Queries;

public sealed record GetTherapistQuery(Guid Id) : IRequest<TherapistDto>;

public sealed class GetTherapistQueryHandler : IRequestHandler<GetTherapistQuery, TherapistDto>
{
    private readonly IApplicationDbContext _context;

    public GetTherapistQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<TherapistDto> Handle(GetTherapistQuery request, CancellationToken cancellationToken)
    {
        var user = await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == request.Id && u.IsTherapist, cancellationToken);

        if (user is null)
        {
            throw new NotFoundException(nameof(User), request.Id);
        }

        return new TherapistDto(
            user.Id,
            user.Email,
            user.FirstName,
            user.LastName,
            user.Phone,
            user.Specialization,
            user.Color,
            user.AvatarUrl,
            user.IsActive,
            user.CreatedAt);
    }
}
