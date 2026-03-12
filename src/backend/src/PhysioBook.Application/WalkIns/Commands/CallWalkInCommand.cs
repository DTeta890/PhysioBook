using MediatR;
using Microsoft.EntityFrameworkCore;
using PhysioBook.Application.Common.Exceptions;
using PhysioBook.Application.Common.Interfaces;
using PhysioBook.Application.WalkIns.DTOs;
using PhysioBook.Domain.Entities;

namespace PhysioBook.Application.WalkIns.Commands;

public sealed record CallWalkInCommand(
    Guid Id,
    Guid TherapistId) : IRequest<WalkInEntryDto>;

public sealed class CallWalkInCommandHandler : IRequestHandler<CallWalkInCommand, WalkInEntryDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IDateTimeProvider _dateTime;

    public CallWalkInCommandHandler(IApplicationDbContext context, IDateTimeProvider dateTime)
    {
        _context = context;
        _dateTime = dateTime;
    }

    public async Task<WalkInEntryDto> Handle(CallWalkInCommand request, CancellationToken cancellationToken)
    {
        var entry = await _context.WalkInEntries
            .FirstOrDefaultAsync(w => w.Id == request.Id, cancellationToken);

        if (entry is null)
        {
            throw new NotFoundException(nameof(WalkInEntry), request.Id);
        }

        if (entry.Status != "waiting")
        {
            throw new ValidationException(new Dictionary<string, string[]>
            {
                { "Status", ["Walk-in entry must be in 'waiting' status to be called."] }
            });
        }

        var therapist = await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == request.TherapistId && u.IsTherapist, cancellationToken);

        if (therapist is null)
        {
            throw new NotFoundException("Therapist", request.TherapistId);
        }

        var now = _dateTime.Now;
        entry.Status = "in_progress";
        entry.CalledAt = now;
        entry.AssignedTherapistId = request.TherapistId;
        entry.UpdatedAt = now;

        await _context.SaveChangesAsync(cancellationToken);

        string? treatmentTypeName = null;
        if (entry.TreatmentTypeId.HasValue)
        {
            var treatmentType = await _context.TreatmentTypes
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Id == entry.TreatmentTypeId.Value, cancellationToken);
            treatmentTypeName = treatmentType?.Name;
        }

        var waitMinutes = (entry.CalledAt!.Value - entry.CheckedInAt).TotalMinutes;

        return new WalkInEntryDto(
            entry.Id,
            entry.PatientId,
            entry.PatientName,
            entry.PatientPhone,
            entry.TreatmentTypeId,
            treatmentTypeName,
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
