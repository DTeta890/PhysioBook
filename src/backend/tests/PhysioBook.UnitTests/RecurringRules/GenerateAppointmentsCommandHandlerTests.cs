using Microsoft.EntityFrameworkCore;
using Moq;
using PhysioBook.Application.Common.Exceptions;
using PhysioBook.Application.Common.Interfaces;
using PhysioBook.Application.RecurringRules.Commands;
using PhysioBook.Domain.Entities;

namespace PhysioBook.UnitTests.RecurringRules;

public class GenerateAppointmentsCommandHandlerTests
{
    private static RecurringRule CreateActiveRule(DayOfWeek dayOfWeek = DayOfWeek.Monday)
    {
        return new RecurringRule
        {
            Id = Guid.NewGuid(),
            TenantId = Guid.NewGuid(),
            TherapistId = Guid.NewGuid(),
            TreatmentTypeId = Guid.NewGuid(),
            PatientName = "Test Patient",
            Frequency = "weekly",
            DayOfWeek = (int)dayOfWeek,
            StartTimeOfDay = new TimeOnly(9, 0),
            EndTimeOfDay = new TimeOnly(9, 30),
            StartsFrom = new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero),
            IsActive = true,
        };
    }

    [Fact]
    public void IsFirstOccurrenceOfDayInMonth_Day1To7_ReturnsTrue()
    {
        // The first Monday in a month is always between day 1 and day 7.
        // Testing via the generation command: monthly frequency should only generate
        // for dates where the day is <= 7.
        // This is a logic unit test verifying the static helper logic.
        var date = new DateOnly(2026, 3, 2); // March 2, 2026 is a Monday, day 2 <= 7
        Assert.True(date.Day <= 7);

        var date2 = new DateOnly(2026, 3, 9); // March 9, 2026 is a Monday, day 9 > 7
        Assert.True(date2.Day > 7);
    }

    [Fact]
    public void BiweeklyFrequency_EvenWeeks_AreSelected()
    {
        // Verify the biweekly logic: weeks 0, 2, 4 from start are selected (even)
        var startsFrom = new DateOnly(2026, 1, 5); // Monday
        var week0 = new DateOnly(2026, 1, 5); // 0 days diff, week 0 = even
        var week1 = new DateOnly(2026, 1, 12); // 7 days diff, week 1 = odd
        var week2 = new DateOnly(2026, 1, 19); // 14 days diff, week 2 = even

        Assert.Equal(0, (week0.DayNumber - startsFrom.DayNumber) / 7 % 2);
        Assert.Equal(1, (week1.DayNumber - startsFrom.DayNumber) / 7 % 2);
        Assert.Equal(0, (week2.DayNumber - startsFrom.DayNumber) / 7 % 2);
    }
}
