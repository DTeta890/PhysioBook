using MediatR;
using Microsoft.EntityFrameworkCore;
using PhysioBook.Application.Common.Exceptions;
using PhysioBook.Application.Common.Interfaces;
using PhysioBook.Application.TreatmentTypes.DTOs;
using PhysioBook.Domain.Entities;

namespace PhysioBook.Application.TreatmentTypes.Queries;

public sealed record GetTreatmentTypeQuery(Guid Id) : IRequest<TreatmentTypeDto>;

public sealed class GetTreatmentTypeQueryHandler : IRequestHandler<GetTreatmentTypeQuery, TreatmentTypeDto>
{
    private readonly IApplicationDbContext _context;

    public GetTreatmentTypeQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<TreatmentTypeDto> Handle(GetTreatmentTypeQuery request, CancellationToken cancellationToken)
    {
        var entity = await _context.TreatmentTypes
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == request.Id, cancellationToken);

        if (entity is null)
        {
            throw new NotFoundException(nameof(TreatmentType), request.Id);
        }

        return new TreatmentTypeDto(
            entity.Id,
            entity.Name,
            entity.Description,
            entity.DurationMinutes,
            entity.Price,
            entity.Color,
            entity.IsActive,
            entity.CreatedAt);
    }
}
