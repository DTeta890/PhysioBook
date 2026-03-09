namespace PhysioBook.Application.Common.Interfaces;

public interface IDateTimeProvider
{
    DateTimeOffset Now { get; }
    string TimeZone { get; }
}
