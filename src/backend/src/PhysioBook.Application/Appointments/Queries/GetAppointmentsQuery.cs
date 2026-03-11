using MediatR;
using Microsoft.EntityFrameworkCore;
using PhysioBook.Application.Appointments.DTOs;
using PhysioBook.Application.Common.Interfaces;

namespace PhysioBook.Application.Appointments.Queries;

public sealed record GetAppointmentsQuery(
    DateTimeOffset StartDate,
    DateTimeOffset EndDate,
    Guid? TherapistId) : IRequest<List<AppointmentDto>>;

public sealed class GetAppointmentsQueryHandler : IRequestHandler<GetAppointmentsQuery, List<AppointmentDto>>
{
    private readonly IApplicationDbContext _context;

    public GetAppointmentsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<AppointmentDto>> Handle(GetAppointmentsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Appointments
            .AsNoTracking()
            .Include(a => a.Therapist)
            .Include(a => a.TreatmentType)
            .Where(a => a.StartTime >= request.StartDate && a.StartTime < request.EndDate);

        if (request.TherapistId.HasValue)
        {
            query = query.Where(a => a.TherapistId == request.TherapistId.Value);
        }

        return await query
            .OrderBy(a => a.StartTime)
            .Select(a => new AppointmentDto(
                a.Id,
                a.TherapistId,
                a.Therapist.FirstName + " " + a.Therapist.LastName,
                a.PatientId,
                a.PatientName,
                a.PatientPhone,
                a.TreatmentTypeId,
                a.TreatmentType.Name,
                a.StartTime,
                a.EndTime,
                a.Status,
                a.Notes,
                a.CancellationReason,
                a.IsWalkIn,
                a.Color,
                a.CreatedAt))
            .ToListAsync(cancellationToken);
    }
}
