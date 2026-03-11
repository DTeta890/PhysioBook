using PhysioBook.Application.Appointments.DTOs;

namespace PhysioBook.Application.Common.Interfaces;

public interface ICalendarNotificationService
{
    Task NotifyAppointmentCreated(Guid tenantId, AppointmentDto appointment, CancellationToken cancellationToken = default);
    Task NotifyAppointmentUpdated(Guid tenantId, AppointmentDto appointment, CancellationToken cancellationToken = default);
    Task NotifyAppointmentDeleted(Guid tenantId, Guid appointmentId, CancellationToken cancellationToken = default);
    Task NotifyAppointmentsGenerated(Guid tenantId, List<AppointmentDto> appointments, CancellationToken cancellationToken = default);
}
