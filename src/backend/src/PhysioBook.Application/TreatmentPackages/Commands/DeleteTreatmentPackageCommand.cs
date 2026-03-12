using MediatR;
using Microsoft.EntityFrameworkCore;
using PhysioBook.Application.Common.Exceptions;
using PhysioBook.Application.Common.Interfaces;
using PhysioBook.Domain.Entities;

namespace PhysioBook.Application.TreatmentPackages.Commands;

public sealed record DeleteTreatmentPackageCommand(Guid Id) : IRequest<Unit>;

public sealed class DeleteTreatmentPackageCommandHandler : IRequestHandler<DeleteTreatmentPackageCommand, Unit>
{
    private readonly IApplicationDbContext _context;

    public DeleteTreatmentPackageCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(DeleteTreatmentPackageCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.TreatmentPackages
            .FirstOrDefaultAsync(t => t.Id == request.Id, cancellationToken);

        if (entity is null)
        {
            throw new NotFoundException(nameof(TreatmentPackage), request.Id);
        }

        // Soft-delete: set IsActive = false
        entity.IsActive = false;

        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
