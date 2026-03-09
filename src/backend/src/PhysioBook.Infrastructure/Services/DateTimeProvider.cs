using PhysioBook.Application.Common.Interfaces;

namespace PhysioBook.Infrastructure.Services;

public class DateTimeProvider : IDateTimeProvider
{
    public DateTimeOffset Now => DateTimeOffset.UtcNow;
    public string TimeZone => "Europe/Tirane";
}
