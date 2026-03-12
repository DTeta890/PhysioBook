using MediatR;
using Microsoft.EntityFrameworkCore;
using PhysioBook.Application.Common.Exceptions;
using PhysioBook.Application.Common.Interfaces;
using PhysioBook.Application.WalkIns.DTOs;
using PhysioBook.Domain.Entities;

namespace PhysioBook.Application.WalkIns.Commands;

public sealed record CancelWalkInCommand(
    Guid Id,
    string? Reason) : IRequest<WalkInEntryDto>;

public sealed class CancelWalkInCommandHandler : IRequestHandler<CancelWalkInCommand, WalkInEntryDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IDateTimeProvider _dateTime;

    public CancelWalkInCommandHandler(IApplicationDbContext context, IDateTimeProvider dateTime)
    {
        _context = context;
        _dateTime = dateTime;
    }

    public async Task<WalkInEntryDto> Handle(CancelWalkInCommand request, CancellationToken cancellationToken)
    {
        var entry = await _context.WalkInEntries
            .FirstOrDefaultAsync(w => w.Id == request.Id, cancellationToken);

        if (entry is null)
        {
            throw new NotFoundException(nameof(WalkInEntry), request.Id);
        }

        if (entry.Status != "waiting" && entry.Status != "in_progress")
        {
            throw new ValidationException(new Dictionary<string, string[]>
            {
                { "Status", ["Walk-in entry must be in 'waiting' or 'in_progress' status to be cancelled."] }
            });
        }

        var now = _dateTime.Now;
        entry.Status = "cancelled";
        entry.CompletedAt = now;
        entry.Notes = string.IsNullOrWhiteSpace(request.Reason)
            ? entry.Notes
            : $"{entry.Notes}{(string.IsNullOrWhiteSpace(entry.Notes) ? "" : " | ")}Cancellation: {request.Reason}";
        entry.UpdatedAt = now;

        await _context.SaveChangesAsync(cancellationToken);

        string? therapistName = null;
        if (entry.AssignedTherapistId.HasValue)
        {
            var therapist = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == entry.AssignedTherapistId.Value, cancellationToken);
            therapistName = therapist?.FullName;
        }

        string? treatmentTypeName = null;
        if (entry.TreatmentTypeId.HasValue)
        {
            var treatmentType = await _context.TreatmentTypes
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Id == entry.TreatmentTypeId.Value, cancellationToken);
            treatmentTypeName = treatmentType?.Name;
        }

        var waitMinutes = ((entry.CalledAt ?? now) - entry.CheckedInAt).TotalMinutes;

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
            therapistName,
            entry.ConvertedAppointmentId,
            entry.QueuePosition,
            waitMinutes,
            entry.Notes,
            entry.CreatedAt);
    }
}
