using MediatR;
using Microsoft.EntityFrameworkCore;
using PhysioBook.Application.Common.Exceptions;
using PhysioBook.Application.Common.Interfaces;
using PhysioBook.Application.PatientPackages.DTOs;
using PhysioBook.Domain.Entities;

namespace PhysioBook.Application.PatientPackages.Queries;

public sealed record GetPatientPackageQuery(Guid Id) : IRequest<PatientPackageDto>;

public sealed class GetPatientPackageQueryHandler : IRequestHandler<GetPatientPackageQuery, PatientPackageDto>
{
    private readonly IApplicationDbContext _context;

    public GetPatientPackageQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PatientPackageDto> Handle(GetPatientPackageQuery request, CancellationToken cancellationToken)
    {
        var entity = await _context.PatientPackages
            .AsNoTracking()
            .Include(pp => pp.TreatmentPackage)
                .ThenInclude(tp => tp.TreatmentType)
            .FirstOrDefaultAsync(pp => pp.Id == request.Id, cancellationToken);

        if (entity is null)
        {
            throw new NotFoundException(nameof(PatientPackage), request.Id);
        }

        var tp = entity.TreatmentPackage;
        return new PatientPackageDto(
            entity.Id,
            entity.PatientId,
            entity.TreatmentPackageId,
            tp.Name,
            tp.TreatmentType.Name,
            tp.TotalSessions,
            entity.SessionsUsed,
            tp.TotalSessions - entity.SessionsUsed,
            tp.Price,
            entity.PurchasedAt,
            entity.ExpiresAt,
            entity.Status,
            entity.Notes,
            entity.CreatedAt);
    }
}
