using MediatR;
using Microsoft.EntityFrameworkCore;
using PhysioBook.Application.Appointments.DTOs;
using PhysioBook.Application.Common.Exceptions;
using PhysioBook.Application.Common.Interfaces;
using PhysioBook.Domain.Entities;

namespace PhysioBook.Application.Appointments.Commands;

public sealed record UpdateAppointmentStatusCommand(
    Guid Id,
    string Status,
    string? CancellationReason) : IRequest<AppointmentDto>;

public sealed class UpdateAppointmentStatusCommandHandler : IRequestHandler<UpdateAppointmentStatusCommand, AppointmentDto>
{
    private static readonly Dictionary<string, HashSet<string>> ValidTransitions = new()
    {
        ["scheduled"] = ["confirmed", "in_progress", "cancelled", "no_show"],
        ["confirmed"] = ["in_progress", "cancelled", "no_show"],
        ["in_progress"] = ["completed", "cancelled"],
        ["completed"] = [],
        ["cancelled"] = [],
        ["no_show"] = [],
    };

    private readonly IApplicationDbContext _context;
    private readonly ICalendarNotificationService _notificationService;
    private readonly ICurrentTenantService _tenantService;

    public UpdateAppointmentStatusCommandHandler(
        IApplicationDbContext context,
        ICalendarNotificationService notificationService,
        ICurrentTenantService tenantService)
    {
        _context = context;
        _notificationService = notificationService;
        _tenantService = tenantService;
    }

    public async Task<AppointmentDto> Handle(UpdateAppointmentStatusCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.Appointments
            .Include(a => a.Therapist)
            .Include(a => a.TreatmentType)
            .FirstOrDefaultAsync(a => a.Id == request.Id, cancellationToken);

        if (entity is null)
        {
            throw new NotFoundException(nameof(Appointment), request.Id);
        }

        if (!ValidTransitions.TryGetValue(entity.Status, out var allowedStatuses) ||
            !allowedStatuses.Contains(request.Status))
        {
            throw new ConflictException(
                $"Cannot transition from '{entity.Status}' to '{request.Status}'.");
        }

        entity.Status = request.Status;

        if (request.Status == "cancelled")
        {
            entity.CancellationReason = request.CancellationReason;
        }

        await _context.SaveChangesAsync(cancellationToken);

        var dto = new AppointmentDto(
            entity.Id,
            entity.TherapistId,
            entity.Therapist.FullName,
            entity.PatientId,
            entity.PatientName,
            entity.PatientPhone,
            entity.TreatmentTypeId,
            entity.TreatmentType.Name,
            entity.StartTime,
            entity.EndTime,
            entity.Status,
            entity.Notes,
            entity.CancellationReason,
            entity.IsWalkIn,
            entity.Color,
            entity.RecurringRuleId,
            entity.CreatedAt);

        await _notificationService.NotifyAppointmentUpdated(_tenantService.TenantId, dto, cancellationToken);

        return dto;
    }
}
