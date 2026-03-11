using MediatR;
using Microsoft.EntityFrameworkCore;
using PhysioBook.Application.Common.Exceptions;
using PhysioBook.Application.Common.Interfaces;
using PhysioBook.Domain.Entities;

namespace PhysioBook.Application.RecurringRules.Commands;

public sealed record DeleteRecurringRuleCommand(Guid Id) : IRequest<Unit>;

public sealed class DeleteRecurringRuleCommandHandler : IRequestHandler<DeleteRecurringRuleCommand, Unit>
{
    private readonly IApplicationDbContext _context;

    public DeleteRecurringRuleCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(DeleteRecurringRuleCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.RecurringRules
            .FirstOrDefaultAsync(r => r.Id == request.Id, cancellationToken);

        if (entity is null)
            throw new NotFoundException(nameof(RecurringRule), request.Id);

        // Soft-delete: set IsActive = false
        entity.IsActive = false;

        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
