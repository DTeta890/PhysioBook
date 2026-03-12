using MediatR;
using Microsoft.EntityFrameworkCore;
using PhysioBook.Application.Common.Exceptions;
using PhysioBook.Application.Common.Interfaces;
using PhysioBook.Application.TreatmentPackages.DTOs;
using PhysioBook.Domain.Entities;

namespace PhysioBook.Application.TreatmentPackages.Queries;

public sealed record GetTreatmentPackageQuery(Guid Id) : IRequest<TreatmentPackageDto>;

public sealed class GetTreatmentPackageQueryHandler : IRequestHandler<GetTreatmentPackageQuery, TreatmentPackageDto>
{
    private readonly IApplicationDbContext _context;

    public GetTreatmentPackageQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<TreatmentPackageDto> Handle(GetTreatmentPackageQuery request, CancellationToken cancellationToken)
    {
        var entity = await _context.TreatmentPackages
            .AsNoTracking()
            .Include(tp => tp.TreatmentType)
            .FirstOrDefaultAsync(tp => tp.Id == request.Id, cancellationToken);

        if (entity is null)
        {
            throw new NotFoundException(nameof(TreatmentPackage), request.Id);
        }

        return new TreatmentPackageDto(
            entity.Id,
            entity.Name,
            entity.TreatmentTypeId,
            entity.TreatmentType.Name,
            entity.TotalSessions,
            entity.Price,
            entity.ValidityDays,
            entity.Description,
            entity.IsActive,
            entity.CreatedAt);
    }
}
