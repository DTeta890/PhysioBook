using MediatR;
using Microsoft.EntityFrameworkCore;
using PhysioBook.Application.Appointments.DTOs;
using PhysioBook.Application.Common.Exceptions;
using PhysioBook.Application.Common.Interfaces;
using PhysioBook.Domain.Entities;

namespace PhysioBook.Application.Appointments.Queries;

public sealed record GetAppointmentQuery(Guid Id) : IRequest<AppointmentDto>;

public sealed class GetAppointmentQueryHandler : IRequestHandler<GetAppointmentQuery, AppointmentDto>
{
    private readonly IApplicationDbContext _context;

    public GetAppointmentQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<AppointmentDto> Handle(GetAppointmentQuery request, CancellationToken cancellationToken)
    {
        var entity = await _context.Appointments
            .AsNoTracking()
            .Include(a => a.Therapist)
            .Include(a => a.TreatmentType)
            .FirstOrDefaultAsync(a => a.Id == request.Id, cancellationToken);

        if (entity is null)
        {
            throw new NotFoundException(nameof(Appointment), request.Id);
        }

        return new AppointmentDto(
            entity.Id,
            entity.TherapistId,
            entity.Therapist.FullName,
            entity.PatientId,
            entity.PatientName,
            entity.PatientPhone,
            entity.TreatmentTypeId,
            entity.TreatmentType.Name,
            entity.StartTime,
            entity.EndTime,
            entity.Status,
            entity.Notes,
            entity.CancellationReason,
            entity.IsWalkIn,
            entity.Color,
            entity.RecurringRuleId,
            entity.CreatedAt);
    }
}
