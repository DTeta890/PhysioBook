using MediatR;
using Microsoft.EntityFrameworkCore;
using PhysioBook.Application.Common.Exceptions;
using PhysioBook.Application.Common.Interfaces;
using PhysioBook.Application.PatientPackages.DTOs;
using PhysioBook.Domain.Entities;

namespace PhysioBook.Application.PatientPackages.Commands;

public sealed record PurchasePackageCommand(
    Guid PatientId,
    Guid TreatmentPackageId,
    string? Notes) : IRequest<PatientPackageDto>;

public sealed class PurchasePackageCommandHandler : IRequestHandler<PurchasePackageCommand, PatientPackageDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IDateTimeProvider _dateTimeProvider;

    public PurchasePackageCommandHandler(IApplicationDbContext context, IDateTimeProvider dateTimeProvider)
    {
        _context = context;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<PatientPackageDto> Handle(PurchasePackageCommand request, CancellationToken cancellationToken)
    {
        var treatmentPackage = await _context.TreatmentPackages
            .AsNoTracking()
            .Include(tp => tp.TreatmentType)
            .FirstOrDefaultAsync(tp => tp.Id == request.TreatmentPackageId, cancellationToken);

        if (treatmentPackage is null)
        {
            throw new NotFoundException(nameof(TreatmentPackage), request.TreatmentPackageId);
        }

        if (!treatmentPackage.IsActive)
        {
            throw new ConflictException("Cannot purchase an inactive treatment package.");
        }

        var now = _dateTimeProvider.Now;
        DateTimeOffset? expiresAt = treatmentPackage.ValidityDays.HasValue
            ? now.AddDays(treatmentPackage.ValidityDays.Value)
            : null;

        var entity = new PatientPackage
        {
            PatientId = request.PatientId,
            TreatmentPackageId = request.TreatmentPackageId,
            SessionsUsed = 0,
            PurchasedAt = now,
            ExpiresAt = expiresAt,
            Status = "active",
            Notes = request.Notes,
        };

        _context.PatientPackages.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return new PatientPackageDto(
            entity.Id,
            entity.PatientId,
            entity.TreatmentPackageId,
            treatmentPackage.Name,
            treatmentPackage.TreatmentType.Name,
            treatmentPackage.TotalSessions,
            entity.SessionsUsed,
            treatmentPackage.TotalSessions - entity.SessionsUsed,
            treatmentPackage.Price,
            entity.PurchasedAt,
            entity.ExpiresAt,
            entity.Status,
            entity.Notes,
            entity.CreatedAt);
    }
}
