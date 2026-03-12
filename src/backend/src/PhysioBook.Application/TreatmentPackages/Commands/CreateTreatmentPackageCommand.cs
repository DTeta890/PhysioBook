using MediatR;
using Microsoft.EntityFrameworkCore;
using PhysioBook.Application.Common.Exceptions;
using PhysioBook.Application.Common.Interfaces;
using PhysioBook.Application.TreatmentPackages.DTOs;
using PhysioBook.Domain.Entities;

namespace PhysioBook.Application.TreatmentPackages.Commands;

public sealed record CreateTreatmentPackageCommand(
    string Name,
    Guid TreatmentTypeId,
    int TotalSessions,
    decimal Price,
    int? ValidityDays,
    string? Description) : IRequest<TreatmentPackageDto>;

public sealed class CreateTreatmentPackageCommandHandler : IRequestHandler<CreateTreatmentPackageCommand, TreatmentPackageDto>
{
    private readonly IApplicationDbContext _context;

    public CreateTreatmentPackageCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<TreatmentPackageDto> Handle(CreateTreatmentPackageCommand request, CancellationToken cancellationToken)
    {
        var treatmentType = await _context.TreatmentTypes
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == request.TreatmentTypeId, cancellationToken);

        if (treatmentType is null)
        {
            throw new NotFoundException(nameof(TreatmentType), request.TreatmentTypeId);
        }

        var entity = new TreatmentPackage
        {
            Name = request.Name,
            TreatmentTypeId = request.TreatmentTypeId,
            TotalSessions = request.TotalSessions,
            Price = request.Price,
            ValidityDays = request.ValidityDays,
            Description = request.Description,
            IsActive = true,
        };

        _context.TreatmentPackages.Add(entity);
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
