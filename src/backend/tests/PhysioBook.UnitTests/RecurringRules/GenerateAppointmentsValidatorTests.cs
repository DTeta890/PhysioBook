using FluentValidation.TestHelper;
using PhysioBook.Application.RecurringRules.Commands;
using PhysioBook.Application.RecurringRules.Validators;

namespace PhysioBook.UnitTests.RecurringRules;

public class GenerateAppointmentsValidatorTests
{
    private readonly GenerateAppointmentsValidator _sut = new();

    private static GenerateAppointmentsCommand ValidCommand() => new(
        RecurringRuleId: Guid.NewGuid(),
        FromDate: DateTimeOffset.UtcNow,
        ToDate: DateTimeOffset.UtcNow.AddMonths(1));

    [Fact]
    public void Valid_Command_ShouldPass()
    {
        var result = _sut.TestValidate(ValidCommand());
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Empty_RecurringRuleId_ShouldFail()
    {
        var command = ValidCommand() with { RecurringRuleId = Guid.Empty };
        var result = _sut.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.RecurringRuleId);
    }

    [Fact]
    public void ToDate_Before_FromDate_ShouldFail()
    {
        var now = DateTimeOffset.UtcNow;
        var command = ValidCommand() with { FromDate = now, ToDate = now.AddDays(-1) };
        var result = _sut.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.ToDate);
    }

    [Fact]
    public void ToDate_Equal_FromDate_ShouldFail()
    {
        var now = DateTimeOffset.UtcNow;
        var command = ValidCommand() with { FromDate = now, ToDate = now };
        var result = _sut.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.ToDate);
    }

    [Fact]
    public void DateRange_Exceeding_366_Days_ShouldFail()
    {
        var now = DateTimeOffset.UtcNow;
        var command = ValidCommand() with { FromDate = now, ToDate = now.AddDays(367) };
        var result = _sut.TestValidate(command);
        result.ShouldHaveAnyValidationError();
    }

    [Fact]
    public void DateRange_Exactly_366_Days_ShouldPass()
    {
        var now = DateTimeOffset.UtcNow;
        var command = ValidCommand() with { FromDate = now, ToDate = now.AddDays(366) };
        var result = _sut.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
