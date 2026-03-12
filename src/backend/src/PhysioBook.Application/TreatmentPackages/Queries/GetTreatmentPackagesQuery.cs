using MediatR;
using Microsoft.EntityFrameworkCore;
using PhysioBook.Application.Common.Interfaces;
using PhysioBook.Application.TreatmentPackages.DTOs;

namespace PhysioBook.Application.TreatmentPackages.Queries;

public sealed record GetTreatmentPackagesQuery(
    Guid? TreatmentTypeId = null,
    bool? IsActive = null) : IRequest<List<TreatmentPackageDto>>;

public sealed class GetTreatmentPackagesQueryHandler : IRequestHandler<GetTreatmentPackagesQuery, List<TreatmentPackageDto>>
{
    private readonly IApplicationDbContext _context;

    public GetTreatmentPackagesQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<TreatmentPackageDto>> Handle(GetTreatmentPackagesQuery request, CancellationToken cancellationToken)
    {
        var query = _context.TreatmentPackages
            .AsNoTracking()
            .Include(tp => tp.TreatmentType)
            .AsQueryable();

        if (request.TreatmentTypeId.HasValue)
        {
            query = query.Where(tp => tp.TreatmentTypeId == request.TreatmentTypeId.Value);
        }

        if (request.IsActive.HasValue)
        {
            query = query.Where(tp => tp.IsActive == request.IsActive.Value);
        }

        return await query
            .OrderBy(tp => tp.Name)
            .Select(tp => new TreatmentPackageDto(
                tp.Id,
                tp.Name,
                tp.TreatmentTypeId,
                tp.TreatmentType.Name,
                tp.TotalSessions,
                tp.Price,
                tp.ValidityDays,
                tp.Description,
                tp.IsActive,
                tp.CreatedAt))
            .ToListAsync(cancellationToken);
    }
}
