using MediatR;
using Microsoft.EntityFrameworkCore;
using PhysioBook.Application.Common.Interfaces;

namespace PhysioBook.Application.Appointments.Queries;

public sealed record GetTherapistAvailabilityQuery(
    Guid TherapistId,
    DateOnly Date) : IRequest<TherapistAvailabilityResult>;

public sealed record TherapistAvailabilityResult(
    Guid TherapistId,
    DateOnly Date,
    List<TimeSlotDto> BusySlots);

public sealed record TimeSlotDto(
    DateTimeOffset StartTime,
    DateTimeOffset EndTime,
    string? PatientName,
    string Status);

public sealed class GetTherapistAvailabilityQueryHandler : IRequestHandler<GetTherapistAvailabilityQuery, TherapistAvailabilityResult>
{
    private readonly IApplicationDbContext _context;

    public GetTherapistAvailabilityQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<TherapistAvailabilityResult> Handle(GetTherapistAvailabilityQuery request, CancellationToken cancellationToken)
    {
        var dayStart = new DateTimeOffset(request.Date.Year, request.Date.Month, request.Date.Day, 0, 0, 0, TimeSpan.Zero);
        var dayEnd = dayStart.AddDays(1);

        var busySlots = await _context.Appointments
            .AsNoTracking()
            .Where(a =>
                a.TherapistId == request.TherapistId &&
                a.Status != "cancelled" &&
                a.Status != "no_show" &&
                a.StartTime < dayEnd &&
                a.EndTime > dayStart)
            .OrderBy(a => a.StartTime)
            .Select(a => new TimeSlotDto(
                a.StartTime,
                a.EndTime,
                a.PatientName,
                a.Status))
            .ToListAsync(cancellationToken);

        return new TherapistAvailabilityResult(
            request.TherapistId,
            request.Date,
            busySlots);
    }
}
