using FluentValidation.TestHelper;
using PhysioBook.Application.Appointments.Queries;
using PhysioBook.Application.Appointments.Validators;

namespace PhysioBook.UnitTests.Appointments;

public class CheckConflictsValidatorTests
{
    private readonly CheckConflictsValidator _sut = new();

    private static CheckConflictsQuery ValidQuery() => new(
        TherapistId: Guid.NewGuid(),
        StartTime: DateTimeOffset.UtcNow.AddHours(1),
        EndTime: DateTimeOffset.UtcNow.AddHours(2));

    [Fact]
    public void Valid_Query_ShouldPass()
    {
        var result = _sut.TestValidate(ValidQuery());
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Empty_TherapistId_ShouldFail()
    {
        var query = ValidQuery() with { TherapistId = Guid.Empty };
        var result = _sut.TestValidate(query);
        result.ShouldHaveValidationErrorFor(x => x.TherapistId);
    }

    [Fact]
    public void StartTime_After_EndTime_ShouldFail()
    {
        var now = DateTimeOffset.UtcNow;
        var query = ValidQuery() with { StartTime = now.AddHours(2), EndTime = now.AddHours(1) };
        var result = _sut.TestValidate(query);
        result.ShouldHaveValidationErrorFor(x => x.EndTime);
    }

    [Fact]
    public void StartTime_Equal_EndTime_ShouldFail()
    {
        var now = DateTimeOffset.UtcNow.AddHours(1);
        var query = ValidQuery() with { StartTime = now, EndTime = now };
        var result = _sut.TestValidate(query);
        result.ShouldHaveValidationErrorFor(x => x.EndTime);
    }
}
