using MediatR;
using Microsoft.EntityFrameworkCore;
using PhysioBook.Application.Appointments.DTOs;
using PhysioBook.Application.Common.Exceptions;
using PhysioBook.Application.Common.Interfaces;
using PhysioBook.Domain.Entities;

namespace PhysioBook.Application.Appointments.Commands;

public sealed record CreateAppointmentCommand(
    Guid TherapistId,
    Guid? PatientId,
    string? PatientName,
    string? PatientPhone,
    Guid TreatmentTypeId,
    DateTimeOffset StartTime,
    DateTimeOffset EndTime,
    string? Notes,
    bool IsWalkIn,
    string? Color) : IRequest<AppointmentDto>;

public sealed class CreateAppointmentCommandHandler : IRequestHandler<CreateAppointmentCommand, AppointmentDto>
{
    private readonly IApplicationDbContext _context;

    public CreateAppointmentCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<AppointmentDto> Handle(CreateAppointmentCommand request, CancellationToken cancellationToken)
    {
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

        var entity = new Appointment
        {
            TherapistId = request.TherapistId,
            PatientId = request.PatientId,
            PatientName = request.PatientName,
            PatientPhone = request.PatientPhone,
            TreatmentTypeId = request.TreatmentTypeId,
            StartTime = request.StartTime,
            EndTime = request.EndTime,
            Status = "scheduled",
            Notes = request.Notes,
            IsWalkIn = request.IsWalkIn,
            Color = request.Color,
        };

        _context.Appointments.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return new AppointmentDto(
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
            entity.CreatedAt);
    }
}
