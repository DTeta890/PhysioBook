using MediatR;
using Microsoft.EntityFrameworkCore;
using PhysioBook.Application.Appointments.DTOs;
using PhysioBook.Application.Common.Exceptions;
using PhysioBook.Application.Common.Interfaces;
using PhysioBook.Domain.Entities;

namespace PhysioBook.Application.Appointments.Commands;

public sealed record UpdateAppointmentCommand(
    Guid Id,
    Guid TherapistId,
    Guid? PatientId,
    string? PatientName,
    string? PatientPhone,
    Guid TreatmentTypeId,
    DateTimeOffset StartTime,
    DateTimeOffset EndTime,
    string? Notes,
    string? Color) : IRequest<AppointmentDto>;

public sealed class UpdateAppointmentCommandHandler : IRequestHandler<UpdateAppointmentCommand, AppointmentDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ICalendarNotificationService _notificationService;
    private readonly ICurrentTenantService _tenantService;

    public UpdateAppointmentCommandHandler(
        IApplicationDbContext context,
        ICalendarNotificationService notificationService,
        ICurrentTenantService tenantService)
    {
        _context = context;
        _notificationService = notificationService;
        _tenantService = tenantService;
    }

    public async Task<AppointmentDto> Handle(UpdateAppointmentCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.Appointments
            .FirstOrDefaultAsync(a => a.Id == request.Id, cancellationToken);

        if (entity is null)
        {
            throw new NotFoundException(nameof(Appointment), request.Id);
        }

        var therapist = await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == request.TherapistId && u.IsTherapist, cancellationToken);

        if (therapist is null)
        {
            throw new NotFoundException("Therapist", request.TherapistId);
        }

        var treatmentType = await _context.TreatmentTypes
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == request.TreatmentTypeId, cancellationToken);

        if (treatmentType is null)
        {
            throw new NotFoundException(nameof(TreatmentType), request.TreatmentTypeId);
        }

        var hasConflict = await _context.Appointments
            .AnyAsync(a =>
                a.Id != request.Id &&
                a.TherapistId == request.TherapistId &&
                a.Status != "cancelled" &&
                a.Status != "no_show" &&
                a.StartTime < request.EndTime &&
                a.EndTime > request.StartTime,
                cancellationToken);

        if (hasConflict)
        {
            throw new ConflictException("The therapist already has an appointment in this time range.");
        }

        entity.TherapistId = request.TherapistId;
        entity.PatientId = request.PatientId;
        entity.PatientName = request.PatientName;
        entity.PatientPhone = request.PatientPhone;
        entity.TreatmentTypeId = request.TreatmentTypeId;
        entity.StartTime = request.StartTime;
        entity.EndTime = request.EndTime;
        entity.Notes = request.Notes;
        entity.Color = request.Color;

        await _context.SaveChangesAsync(cancellationToken);

        var dto = new AppointmentDto(
            entity.Id,
            entity.TherapistId,
            therapist.FullName,
            entity.PatientId,
            entity.PatientName,
            entity.PatientPhone,
            entity.TreatmentTypeId,
            treatmentType.Name,
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
