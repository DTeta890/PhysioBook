using MediatR;
using Microsoft.EntityFrameworkCore;
using PhysioBook.Application.Common.Exceptions;
using PhysioBook.Application.Common.Interfaces;
using PhysioBook.Application.TreatmentPackages.DTOs;
using PhysioBook.Domain.Entities;

namespace PhysioBook.Application.TreatmentPackages.Commands;

public sealed record UpdateTreatmentPackageCommand(
    Guid Id,
    string Name,
    Guid TreatmentTypeId,
    int TotalSessions,
    decimal Price,
    int? ValidityDays,
    string? Description,
    bool IsActive) : IRequest<TreatmentPackageDto>;

public sealed class UpdateTreatmentPackageCommandHandler : IRequestHandler<UpdateTreatmentPackageCommand, TreatmentPackageDto>
{
    private readonly IApplicationDbContext _context;

    public UpdateTreatmentPackageCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<TreatmentPackageDto> Handle(UpdateTreatmentPackageCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.TreatmentPackages
            .FirstOrDefaultAsync(t => t.Id == request.Id, cancellationToken);

        if (entity is null)
        {
            throw new NotFoundException(nameof(TreatmentPackage), request.Id);
        }

        var treatmentType = await _context.TreatmentTypes
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == request.TreatmentTypeId, cancellationToken);

        if (treatmentType is null)
        {
            throw new NotFoundException(nameof(TreatmentType), request.TreatmentTypeId);
        }

        entity.Name = request.Name;
        entity.TreatmentTypeId = request.TreatmentTypeId;
        entity.TotalSessions = request.TotalSessions;
        entity.Price = request.Price;
        entity.ValidityDays = request.ValidityDays;
        entity.Description = request.Description;
        entity.IsActive = request.IsActive;

        await _context.SaveChangesAsync(cancellationToken);

        return new TreatmentPackageDto(
            entity.Id,
            entity.Name,
            entity.TreatmentTypeId,
            treatmentType.Name,
            entity.TotalSessions,
            entity.Price,
            entity.ValidityDays,
            entity.Description,
            entity.IsActive,
            entity.CreatedAt);
    }
}
