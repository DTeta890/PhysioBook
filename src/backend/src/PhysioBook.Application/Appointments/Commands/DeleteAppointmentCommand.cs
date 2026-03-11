using MediatR;
using Microsoft.EntityFrameworkCore;
using PhysioBook.Application.Common.Exceptions;
using PhysioBook.Application.Common.Interfaces;
using PhysioBook.Domain.Entities;

namespace PhysioBook.Application.Appointments.Commands;

public sealed record DeleteAppointmentCommand(Guid Id) : IRequest<Unit>;

public sealed class DeleteAppointmentCommandHandler : IRequestHandler<DeleteAppointmentCommand, Unit>
{
    private readonly IApplicationDbContext _context;
    private readonly ICalendarNotificationService _notificationService;
    private readonly ICurrentTenantService _tenantService;

    public DeleteAppointmentCommandHandler(
        IApplicationDbContext context,
        ICalendarNotificationService notificationService,
        ICurrentTenantService tenantService)
    {
        _context = context;
        _notificationService = notificationService;
        _tenantService = tenantService;
    }

    public async Task<Unit> Handle(DeleteAppointmentCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.Appointments
            .FirstOrDefaultAsync(a => a.Id == request.Id, cancellationToken);

        if (entity is null)
        {
            throw new NotFoundException(nameof(Appointment), request.Id);
        }

        _context.Appointments.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);

        await _notificationService.NotifyAppointmentDeleted(_tenantService.TenantId, request.Id, cancellationToken);

        return Unit.Value;
    }
}
