using MediatR;
using Microsoft.EntityFrameworkCore;
using PhysioBook.Application.Common.Exceptions;
using PhysioBook.Application.Common.Interfaces;
using PhysioBook.Domain.Entities;

namespace PhysioBook.Application.Patients.Commands;

public sealed record DeletePatientCommand(Guid Id) : IRequest;

public sealed class DeletePatientCommandHandler : IRequestHandler<DeletePatientCommand>
{
    private readonly IApplicationDbContext _context;

    public DeletePatientCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(DeletePatientCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.Patients
            .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

        if (entity is null)
        {
            throw new NotFoundException(nameof(Patient), request.Id);
        }

        entity.IsActive = false;

        await _context.SaveChangesAsync(cancellationToken);
    }
}
