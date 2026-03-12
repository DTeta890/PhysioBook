using MediatR;
using Microsoft.EntityFrameworkCore;
using PhysioBook.Application.Common.Exceptions;
using PhysioBook.Application.Common.Interfaces;
using PhysioBook.Application.WalkIns.DTOs;
using PhysioBook.Domain.Entities;

namespace PhysioBook.Application.WalkIns.Commands;

public sealed record ConvertToAppointmentCommand(
    Guid WalkInId,
    Guid TherapistId,
    Guid TreatmentTypeId,
    DateTimeOffset StartTime,
    DateTimeOffset EndTime,
    string? Notes) : IRequest<WalkInEntryDto>;

public sealed class ConvertToAppointmentCommandHandler : IRequestHandler<ConvertToAppointmentCommand, WalkInEntryDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IDateTimeProvider _dateTime;

    public ConvertToAppointmentCommandHandler(IApplicationDbContext context, IDateTimeProvider dateTime)
    {
        _context = context;
        _dateTime = dateTime;
    }

    public async Task<WalkInEntryDto> Handle(ConvertToAppointmentCommand request, CancellationToken cancellationToken)
    {
        var entry = await _context.WalkInEntries
            .FirstOrDefaultAsync(w => w.Id == request.WalkInId, cancellationToken);

        if (entry is null)
        {
            throw new NotFoundException(nameof(WalkInEntry), request.WalkInId);
        }

        if (entry.Status != "waiting" && entry.Status != "in_progress")
        {
            throw new ValidationException(new Dictionary<string, string[]>
            {
                { "Status", ["Walk-in entry must be in 'waiting' or 'in_progress' status to be converted."] }
            });
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

        var appointment = new Appointment
        {
            TherapistId = request.TherapistId,
            PatientId = entry.PatientId,
            PatientName = entry.PatientName,
            PatientPhone = entry.PatientPhone,
            TreatmentTypeId = request.TreatmentTypeId,
            StartTime = request.StartTime,
            EndTime = request.EndTime,
            Status = "scheduled",
            Notes = request.Notes,
            IsWalkIn = true,
        };

        _context.Appointments.Add(appointment);

        var now = _dateTime.Now;
        entry.ConvertedAppointmentId = appointment.Id;
        entry.AssignedTherapistId = request.TherapistId;
        entry.Status = "served";
        entry.CompletedAt = now;
        entry.CalledAt ??= now;
        entry.UpdatedAt = now;

        await _context.SaveChangesAsync(cancellationToken);

        var waitMinutes = (entry.CalledAt!.Value - entry.CheckedInAt).TotalMinutes;

        return new WalkInEntryDto(
            entry.Id,
            entry.PatientId,
            entry.PatientName,
            entry.PatientPhone,
            entry.TreatmentTypeId,
            treatmentType.Name,
            entry.ReasonForVisit,
            entry.Priority,
            entry.Status,
            entry.CheckedInAt,
            entry.CalledAt,
            entry.CompletedAt,
            entry.AssignedTherapistId,
            therapist.FullName,
            entry.ConvertedAppointmentId,
            entry.QueuePosition,
            waitMinutes,
            entry.Notes,
            entry.CreatedAt);
    }
}
