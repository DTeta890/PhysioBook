using MediatR;
using Microsoft.EntityFrameworkCore;
using PhysioBook.Application.Common.Interfaces;
using PhysioBook.Application.WalkIns.DTOs;

namespace PhysioBook.Application.WalkIns.Queries;

public sealed record GetWalkInQueueQuery(
    string? Status,
    DateTimeOffset? Date) : IRequest<List<WalkInEntryDto>>;

public sealed class GetWalkInQueueQueryHandler : IRequestHandler<GetWalkInQueueQuery, List<WalkInEntryDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IDateTimeProvider _dateTime;

    public GetWalkInQueueQueryHandler(IApplicationDbContext context, IDateTimeProvider dateTime)
    {
        _context = context;
        _dateTime = dateTime;
    }

    public async Task<List<WalkInEntryDto>> Handle(GetWalkInQueueQuery request, CancellationToken cancellationToken)
    {
        var query = _context.WalkInEntries.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.Status))
        {
            query = query.Where(w => w.Status == request.Status);
        }

        var filterDate = request.Date ?? _dateTime.Now;
        var startOfDay = new DateTimeOffset(filterDate.Date, filterDate.Offset);
        var endOfDay = startOfDay.AddDays(1);

        query = query.Where(w => w.CheckedInAt >= startOfDay && w.CheckedInAt < endOfDay);

        var now = _dateTime.Now;

        var entries = await query
            .OrderByDescending(w => w.Priority)
            .ThenBy(w => w.CheckedInAt)
            .Select(w => new WalkInEntryDto(
                w.Id,
                w.PatientId,
                w.PatientName,
                w.PatientPhone,
                w.TreatmentTypeId,
                w.TreatmentType != null ? w.TreatmentType.Name : null,
                w.ReasonForVisit,
                w.Priority,
                w.Status,
                w.CheckedInAt,
                w.CalledAt,
                w.CompletedAt,
                w.AssignedTherapistId,
                w.AssignedTherapist != null ? w.AssignedTherapist.FirstName + " " + w.AssignedTherapist.LastName : null,
                w.ConvertedAppointmentId,
                w.QueuePosition,
                (w.CalledAt != null ? w.CalledAt.Value : now).Subtract(w.CheckedInAt).TotalMinutes,
                w.Notes,
                w.CreatedAt))
            .ToListAsync(cancellationToken);

        return entries;
    }
}
