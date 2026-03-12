using MediatR;
using Microsoft.EntityFrameworkCore;
using PhysioBook.Application.Common.Exceptions;
using PhysioBook.Application.Common.Interfaces;
using PhysioBook.Application.WalkIns.DTOs;
using PhysioBook.Domain.Entities;

namespace PhysioBook.Application.WalkIns.Commands;

public sealed record CheckInWalkInCommand(
    Guid? PatientId,
    string PatientName,
    string? PatientPhone,
    Guid? TreatmentTypeId,
    string? ReasonForVisit,
    int? Priority,
    string? Notes) : IRequest<WalkInEntryDto>;

public sealed class CheckInWalkInCommandHandler : IRequestHandler<CheckInWalkInCommand, WalkInEntryDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IDateTimeProvider _dateTime;

    public CheckInWalkInCommandHandler(IApplicationDbContext context, IDateTimeProvider dateTime)
    {
        _context = context;
        _dateTime = dateTime;
    }

    public async Task<WalkInEntryDto> Handle(CheckInWalkInCommand request, CancellationToken cancellationToken)
    {
        var patientName = request.PatientName;
        var patientPhone = request.PatientPhone;

        if (request.PatientId.HasValue)
        {
            var user = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == request.PatientId.Value, cancellationToken);

            if (user is null)
            {
                throw new NotFoundException("Patient", request.PatientId.Value);
            }

            patientName = user.FullName;
            patientPhone = user.Phone ?? request.PatientPhone;
        }

        string? treatmentTypeName = null;
        if (request.TreatmentTypeId.HasValue)
        {
            var treatmentType = await _context.TreatmentTypes
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Id == request.TreatmentTypeId.Value, cancellationToken);

            if (treatmentType is null)
            {
                throw new NotFoundException(nameof(TreatmentType), request.TreatmentTypeId.Value);
            }

            treatmentTypeName = treatmentType.Name;
        }

        var now = _dateTime.Now;

        var queuePosition = await _context.WalkInEntries
            .CountAsync(w => w.Status == "waiting", cancellationToken) + 1;

        var entity = new WalkInEntry
        {
            PatientId = request.PatientId,
            PatientName = patientName,
            PatientPhone = patientPhone,
            TreatmentTypeId = request.TreatmentTypeId,
            ReasonForVisit = request.ReasonForVisit,
            Priority = request.Priority ?? 1,
            Status = "waiting",
            CheckedInAt = now,
            QueuePosition = queuePosition,
            Notes = request.Notes,
        };

        _context.WalkInEntries.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return new WalkInEntryDto(
            entity.Id,
            entity.PatientId,
            entity.PatientName,
            entity.PatientPhone,
            entity.TreatmentTypeId,
            treatmentTypeName,
            entity.ReasonForVisit,
            entity.Priority,
            entity.Status,
            entity.CheckedInAt,
            entity.CalledAt,
            entity.CompletedAt,
            entity.AssignedTherapistId,
            null,
            entity.ConvertedAppointmentId,
            entity.QueuePosition,
            0,
            entity.Notes,
            entity.CreatedAt);
    }
}
