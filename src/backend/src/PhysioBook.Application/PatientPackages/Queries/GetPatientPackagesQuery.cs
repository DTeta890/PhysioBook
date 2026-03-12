using MediatR;
using Microsoft.EntityFrameworkCore;
using PhysioBook.Application.Common.Interfaces;
using PhysioBook.Application.PatientPackages.DTOs;

namespace PhysioBook.Application.PatientPackages.Queries;

public sealed record GetPatientPackagesQuery(
    Guid PatientId,
    string? Status = null) : IRequest<List<PatientPackageDto>>;

public sealed class GetPatientPackagesQueryHandler : IRequestHandler<GetPatientPackagesQuery, List<PatientPackageDto>>
{
    private readonly IApplicationDbContext _context;

    public GetPatientPackagesQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<PatientPackageDto>> Handle(GetPatientPackagesQuery request, CancellationToken cancellationToken)
    {
        var query = _context.PatientPackages
            .AsNoTracking()
            .Include(pp => pp.TreatmentPackage)
                .ThenInclude(tp => tp.TreatmentType)
            .Where(pp => pp.PatientId == request.PatientId);

        if (!string.IsNullOrWhiteSpace(request.Status))
        {
            query = query.Where(pp => pp.Status == request.Status);
        }

        return await query
            .OrderByDescending(pp => pp.PurchasedAt)
            .Select(pp => new PatientPackageDto(
                pp.Id,
                pp.PatientId,
                pp.TreatmentPackageId,
                pp.TreatmentPackage.Name,
                pp.TreatmentPackage.TreatmentType.Name,
                pp.TreatmentPackage.TotalSessions,
                pp.SessionsUsed,
                pp.TreatmentPackage.TotalSessions - pp.SessionsUsed,
                pp.TreatmentPackage.Price,
                pp.PurchasedAt,
                pp.ExpiresAt,
                pp.Status,
                pp.Notes,
                pp.CreatedAt))
            .ToListAsync(cancellationToken);
    }
}
