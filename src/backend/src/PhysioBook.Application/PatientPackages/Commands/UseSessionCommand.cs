using MediatR;
using Microsoft.EntityFrameworkCore;
using PhysioBook.Application.Common.Exceptions;
using PhysioBook.Application.Common.Interfaces;
using PhysioBook.Application.PatientPackages.DTOs;
using PhysioBook.Domain.Entities;

namespace PhysioBook.Application.PatientPackages.Commands;

public sealed record UseSessionCommand(Guid Id) : IRequest<PatientPackageDto>;

public sealed class UseSessionCommandHandler : IRequestHandler<UseSessionCommand, PatientPackageDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IDateTimeProvider _dateTimeProvider;

    public UseSessionCommandHandler(IApplicationDbContext context, IDateTimeProvider dateTimeProvider)
    {
        _context = context;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<PatientPackageDto> Handle(UseSessionCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.PatientPackages
            .Include(pp => pp.TreatmentPackage)
                .ThenInclude(tp => tp.TreatmentType)
            .FirstOrDefaultAsync(pp => pp.Id == request.Id, cancellationToken);

        if (entity is null)
        {
            throw new NotFoundException(nameof(PatientPackage), request.Id);
        }

        if (entity.Status != "active")
        {
            throw new ConflictException($"Cannot use session on a package with status '{entity.Status}'.");
        }

        if (entity.ExpiresAt.HasValue && entity.ExpiresAt.Value < _dateTimeProvider.Now)
        {
            entity.Status = "expired";
            await _context.SaveChangesAsync(cancellationToken);
            throw new ConflictException("This package has expired.");
        }

        if (entity.SessionsUsed >= entity.TreatmentPackage.TotalSessions)
        {
            throw new ConflictException("All sessions in this package have already been used.");
        }

        entity.SessionsUsed++;

        // Auto-complete if all sessions have been used
        if (entity.SessionsUsed >= entity.TreatmentPackage.TotalSessions)
        {
            entity.Status = "completed";
        }

        await _context.SaveChangesAsync(cancellationToken);

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
