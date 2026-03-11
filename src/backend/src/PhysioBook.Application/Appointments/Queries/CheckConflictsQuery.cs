using MediatR;
using Microsoft.EntityFrameworkCore;
using PhysioBook.Application.Common.Interfaces;

namespace PhysioBook.Application.Appointments.Queries;

public sealed record CheckConflictsQuery(
    Guid TherapistId,
    DateTimeOffset StartTime,
    DateTimeOffset EndTime,
    Guid? ExcludeAppointmentId = null) : IRequest<ConflictCheckResult>;

public sealed record ConflictCheckResult(
    bool HasConflict,
    List<ConflictingAppointmentDto> ConflictingAppointments);

public sealed record ConflictingAppointmentDto(
    Guid Id,
    string? PatientName,
    DateTimeOffset StartTime,
    DateTimeOffset EndTime,
    string Status);

public sealed class CheckConflictsQueryHandler : IRequestHandler<CheckConflictsQuery, ConflictCheckResult>
{
    private readonly IApplicationDbContext _context;

    public CheckConflictsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ConflictCheckResult> Handle(CheckConflictsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Appointments
            .AsNoTracking()
            .Where(a =>
                a.TherapistId == request.TherapistId &&
                a.Status != "cancelled" &&
                a.Status != "no_show" &&
                a.StartTime < request.EndTime &&
                a.EndTime > request.StartTime);

        if (request.ExcludeAppointmentId.HasValue)
        {
            query = query.Where(a => a.Id != request.ExcludeAppointmentId.Value);
        }

        var conflicting = await query
            .OrderBy(a => a.StartTime)
            .Select(a => new ConflictingAppointmentDto(
                a.Id,
                a.PatientName,
                a.StartTime,
                a.EndTime,
                a.Status))
            .ToListAsync(cancellationToken);

        return new ConflictCheckResult(
            conflicting.Count > 0,
            conflicting);
    }
}
