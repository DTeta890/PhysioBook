using MediatR;
using Microsoft.EntityFrameworkCore;
using PhysioBook.Application.Common.Exceptions;
using PhysioBook.Application.Common.Interfaces;
using PhysioBook.Domain.Entities;

namespace PhysioBook.Application.TreatmentTypes.Commands;

public sealed record DeleteTreatmentTypeCommand(Guid Id) : IRequest<Unit>;

public sealed class DeleteTreatmentTypeCommandHandler : IRequestHandler<DeleteTreatmentTypeCommand, Unit>
{
    private readonly IApplicationDbContext _context;

    public DeleteTreatmentTypeCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(DeleteTreatmentTypeCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.TreatmentTypes
            .FirstOrDefaultAsync(t => t.Id == request.Id, cancellationToken);

        if (entity is null)
        {
            throw new NotFoundException(nameof(TreatmentType), request.Id);
        }

        // Soft-delete: set IsActive = false
        entity.IsActive = false;

        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
