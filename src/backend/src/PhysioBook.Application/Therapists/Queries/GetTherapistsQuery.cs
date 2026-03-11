using MediatR;
using Microsoft.EntityFrameworkCore;
using PhysioBook.Application.Common.Interfaces;
using PhysioBook.Application.Therapists.DTOs;

namespace PhysioBook.Application.Therapists.Queries;

public sealed record GetTherapistsQuery(bool? IsActive = null) : IRequest<List<TherapistDto>>;

public sealed class GetTherapistsQueryHandler : IRequestHandler<GetTherapistsQuery, List<TherapistDto>>
{
    private readonly IApplicationDbContext _context;

    public GetTherapistsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<TherapistDto>> Handle(GetTherapistsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Users
            .AsNoTracking()
            .Where(u => u.IsTherapist);

        if (request.IsActive.HasValue)
        {
            query = query.Where(u => u.IsActive == request.IsActive.Value);
        }

        var therapists = await query
            .OrderBy(u => u.FirstName)
            .ThenBy(u => u.LastName)
            .ToListAsync(cancellationToken);

        return therapists.Select(u => new TherapistDto(
            u.Id,
            u.Email,
            u.FirstName,
            u.LastName,
            u.Phone,
            u.Specialization,
            u.Color,
            u.AvatarUrl,
            u.IsActive,
            u.CreatedAt)).ToList();
    }
}
