using MediatR;
using Microsoft.EntityFrameworkCore;
using PhysioBook.Application.Common.Interfaces;
using PhysioBook.Application.TreatmentTypes.DTOs;

namespace PhysioBook.Application.TreatmentTypes.Queries;

public sealed record GetTreatmentTypesQuery(bool? IsActive = null) : IRequest<List<TreatmentTypeDto>>;

public sealed class GetTreatmentTypesQueryHandler : IRequestHandler<GetTreatmentTypesQuery, List<TreatmentTypeDto>>
{
    private readonly IApplicationDbContext _context;

    public GetTreatmentTypesQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<TreatmentTypeDto>> Handle(GetTreatmentTypesQuery request, CancellationToken cancellationToken)
    {
        var query = _context.TreatmentTypes.AsNoTracking();

        if (request.IsActive.HasValue)
        {
            query = query.Where(t => t.IsActive == request.IsActive.Value);
        }

        return await query
            .OrderBy(t => t.Name)
            .Select(t => new TreatmentTypeDto(
                t.Id,
                t.Name,
                t.Description,
                t.DurationMinutes,
                t.Price,
                t.Color,
                t.IsActive,
                t.CreatedAt))
            .ToListAsync(cancellationToken);
    }
}
