using Microsoft.AspNetCore.SignalR;
using Moq;
using PhysioBook.Api.Hubs;
using PhysioBook.Api.Services;
using PhysioBook.Application.Appointments.DTOs;

namespace PhysioBook.UnitTests.SignalR;

public class CalendarNotificationServiceTests
{
    private readonly Mock<IHubContext<CalendarHub>> _hubContextMock;
    private readonly Mock<IHubClients> _clientsMock;
    private readonly Mock<IClientProxy> _clientProxyMock;
    private readonly CalendarNotificationService _service;
    private readonly Guid _tenantId = Guid.NewGuid();

    public CalendarNotificationServiceTests()
    {
        _hubContextMock = new Mock<IHubContext<CalendarHub>>();
        _clientsMock = new Mock<IHubClients>();
        _clientProxyMock = new Mock<IClientProxy>();

        _hubContextMock.Setup(h => h.Clients).Returns(_clientsMock.Object);
        _clientsMock.Setup(c => c.Group($"tenant-{_tenantId}")).Returns(_clientProxyMock.Object);

        _service = new CalendarNotificationService(_hubContextMock.Object);
    }

    private static AppointmentDto CreateTestDto()
    {
        return new AppointmentDto(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Test Therapist",
            Guid.NewGuid(),
            "Test Patient",
            "+355123456",
            Guid.NewGuid(),
            "Massage",
            DateTimeOffset.UtcNow,
            DateTimeOffset.UtcNow.AddMinutes(30),
            "scheduled",
            null,
            null,
            false,
            "#FF0000",
            null,
            DateTimeOffset.UtcNow);
    }

    [Fact]
    public async Task NotifyAppointmentCreated_SendsToTenantGroup()
    {
        var dto = CreateTestDto();

        await _service.NotifyAppointmentCreated(_tenantId, dto, CancellationToken.None);

        _clientsMock.Verify(c => c.Group($"tenant-{_tenantId}"), Times.Once);
        _clientProxyMock.Verify(
            p => p.SendCoreAsync("AppointmentCreated", It.Is<object?[]>(args => args.Length == 1 && Equals(args[0], dto)), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task NotifyAppointmentUpdated_SendsToTenantGroup()
    {
        var dto = CreateTestDto();

        await _service.NotifyAppointmentUpdated(_tenantId, dto, CancellationToken.None);

        _clientsMock.Verify(c => c.Group($"tenant-{_tenantId}"), Times.Once);
        _clientProxyMock.Verify(
            p => p.SendCoreAsync("AppointmentUpdated", It.Is<object?[]>(args => args.Length == 1 && Equals(args[0], dto)), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task NotifyAppointmentDeleted_SendsAppointmentIdToTenantGroup()
    {
        var appointmentId = Guid.NewGuid();

        await _service.NotifyAppointmentDeleted(_tenantId, appointmentId, CancellationToken.None);

        _clientsMock.Verify(c => c.Group($"tenant-{_tenantId}"), Times.Once);
        _clientProxyMock.Verify(
            p => p.SendCoreAsync("AppointmentDeleted", It.Is<object?[]>(args => args.Length == 1 && (Guid)args[0]! == appointmentId), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task NotifyAppointmentsGenerated_SendsListToTenantGroup()
    {
        var dtos = new List<AppointmentDto> { CreateTestDto(), CreateTestDto() };

        await _service.NotifyAppointmentsGenerated(_tenantId, dtos, CancellationToken.None);

        _clientsMock.Verify(c => c.Group($"tenant-{_tenantId}"), Times.Once);
        _clientProxyMock.Verify(
            p => p.SendCoreAsync("AppointmentsGenerated", It.Is<object?[]>(args => args.Length == 1 && args[0] == dtos), It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
