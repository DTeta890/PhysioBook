using Microsoft.AspNetCore.SignalR;
using PhysioBook.Api.Hubs;
using PhysioBook.Application.Appointments.DTOs;
using PhysioBook.Application.Common.Interfaces;

namespace PhysioBook.Api.Services;

public sealed class CalendarNotificationService : ICalendarNotificationService
{
    private readonly IHubContext<CalendarHub> _hubContext;

    public CalendarNotificationService(IHubContext<CalendarHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task NotifyAppointmentCreated(Guid tenantId, AppointmentDto appointment, CancellationToken cancellationToken)
    {
        await _hubContext.Clients.Group($"tenant-{tenantId}")
            .SendAsync("AppointmentCreated", appointment, cancellationToken);
    }

    public async Task NotifyAppointmentUpdated(Guid tenantId, AppointmentDto appointment, CancellationToken cancellationToken)
    {
        await _hubContext.Clients.Group($"tenant-{tenantId}")
            .SendAsync("AppointmentUpdated", appointment, cancellationToken);
    }

    public async Task NotifyAppointmentDeleted(Guid tenantId, Guid appointmentId, CancellationToken cancellationToken)
    {
        await _hubContext.Clients.Group($"tenant-{tenantId}")
            .SendAsync("AppointmentDeleted", appointmentId, cancellationToken);
    }

    public async Task NotifyAppointmentsGenerated(Guid tenantId, List<AppointmentDto> appointments, CancellationToken cancellationToken)
    {
        await _hubContext.Clients.Group($"tenant-{tenantId}")
            .SendAsync("AppointmentsGenerated", appointments, cancellationToken);
    }
}
