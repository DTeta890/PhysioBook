using MediatR;
using Microsoft.EntityFrameworkCore;
using PhysioBook.Application.Common.Exceptions;
using PhysioBook.Application.Common.Interfaces;
using PhysioBook.Application.WalkIns.DTOs;
using PhysioBook.Domain.Entities;

namespace PhysioBook.Application.WalkIns.Queries;

public sealed record GetWalkInEntryQuery(Guid Id) : IRequest<WalkInEntryDto>;

public sealed class GetWalkInEntryQueryHandler : IRequestHandler<GetWalkInEntryQuery, WalkInEntryDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IDateTimeProvider _dateTime;

    public GetWalkInEntryQueryHandler(IApplicationDbContext context, IDateTimeProvider dateTime)
    {
        _context = context;
        _dateTime = dateTime;
    }

    public async Task<WalkInEntryDto> Handle(GetWalkInEntryQuery request, CancellationToken cancellationToken)
    {
        var now = _dateTime.Now;

        var entry = await _context.WalkInEntries
            .AsNoTracking()
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
            .FirstOrDefaultAsync(w => w.Id == request.Id, cancellationToken);

        if (entry is null)
        {
            throw new NotFoundException(nameof(WalkInEntry), request.Id);
        }

        return entry;
    }
}
