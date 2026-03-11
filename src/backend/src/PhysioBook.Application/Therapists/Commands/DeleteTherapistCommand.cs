using MediatR;
using Microsoft.EntityFrameworkCore;
using PhysioBook.Application.Common.Exceptions;
using PhysioBook.Application.Common.Interfaces;
using PhysioBook.Domain.Entities;

namespace PhysioBook.Application.Therapists.Commands;

public sealed record DeleteTherapistCommand(Guid Id) : IRequest;

public sealed class DeleteTherapistCommandHandler : IRequestHandler<DeleteTherapistCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly IDateTimeProvider _dateTimeProvider;

    public DeleteTherapistCommandHandler(
        IApplicationDbContext context,
        IDateTimeProvider dateTimeProvider)
    {
        _context = context;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task Handle(DeleteTherapistCommand request, CancellationToken cancellationToken)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == request.Id && u.IsTherapist, cancellationToken);

        if (user is null)
        {
            throw new NotFoundException(nameof(User), request.Id);
        }

        user.IsActive = false;
        user.UpdatedAt = _dateTimeProvider.Now;

        await _context.SaveChangesAsync(cancellationToken);
    }
}
