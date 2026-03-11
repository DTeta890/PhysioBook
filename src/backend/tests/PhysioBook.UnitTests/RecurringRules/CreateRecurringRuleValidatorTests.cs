using FluentValidation.TestHelper;
using PhysioBook.Application.RecurringRules.Commands;
using PhysioBook.Application.RecurringRules.Validators;

namespace PhysioBook.UnitTests.RecurringRules;

public class CreateRecurringRuleValidatorTests
{
    private readonly CreateRecurringRuleValidator _sut = new();

    private static CreateRecurringRuleCommand ValidCommand() => new(
        TherapistId: Guid.NewGuid(),
        PatientId: null,
        PatientName: "John Doe",
        PatientPhone: "+355691234567",
        TreatmentTypeId: Guid.NewGuid(),
        Frequency: "weekly",
        DayOfWeek: 1,
        StartTimeOfDay: "09:00",
        EndTimeOfDay: "09:30",
        StartsFrom: DateTimeOffset.UtcNow,
        EndsAt: DateTimeOffset.UtcNow.AddMonths(3),
        MaxOccurrences: null,
        Notes: "Regular session",
        Color: "#3B82F6");

    [Fact]
    public void Valid_Command_ShouldPass()
    {
        var result = _sut.TestValidate(ValidCommand());
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Empty_TherapistId_ShouldFail()
    {
        var command = ValidCommand() with { TherapistId = Guid.Empty };
        var result = _sut.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.TherapistId);
    }

    [Fact]
    public void Empty_TreatmentTypeId_ShouldFail()
    {
        var command = ValidCommand() with { TreatmentTypeId = Guid.Empty };
        var result = _sut.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.TreatmentTypeId);
    }

    [Theory]
    [InlineData("weekly")]
    [InlineData("biweekly")]
    [InlineData("monthly")]
    public void Valid_Frequency_ShouldPass(string frequency)
    {
        var command = ValidCommand() with { Frequency = frequency };
        var result = _sut.TestValidate(command);
        result.ShouldNotHaveValidationErrorFor(x => x.Frequency);
    }

    [Theory]
    [InlineData("daily")]
    [InlineData("yearly")]
    [InlineData("")]
    [InlineData("invalid")]
    public void Invalid_Frequency_ShouldFail(string frequency)
    {
        var command = ValidCommand() with { Frequency = frequency };
        var result = _sut.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Frequency);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(7)]
    [InlineData(10)]
    public void Invalid_DayOfWeek_ShouldFail(int dayOfWeek)
    {
        var command = ValidCommand() with { DayOfWeek = dayOfWeek };
        var result = _sut.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.DayOfWeek);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(3)]
    [InlineData(6)]
    public void Valid_DayOfWeek_ShouldPass(int dayOfWeek)
    {
        var command = ValidCommand() with { DayOfWeek = dayOfWeek };
        var result = _sut.TestValidate(command);
        result.ShouldNotHaveValidationErrorFor(x => x.DayOfWeek);
    }

    [Theory]
    [InlineData("")]
    [InlineData("invalid")]
    [InlineData("25:00")]
    public void Invalid_StartTimeOfDay_ShouldFail(string time)
    {
        var command = ValidCommand() with { StartTimeOfDay = time };
        var result = _sut.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.StartTimeOfDay);
    }

    [Theory]
    [InlineData("09:00")]
    [InlineData("14:30")]
    [InlineData("00:00")]
    [InlineData("23:59")]
    public void Valid_StartTimeOfDay_ShouldPass(string time)
    {
        var command = ValidCommand() with { StartTimeOfDay = time };
        var result = _sut.TestValidate(command);
        result.ShouldNotHaveValidationErrorFor(x => x.StartTimeOfDay);
    }

    [Fact]
    public void EndTime_Before_StartTime_ShouldFail()
    {
        var command = ValidCommand() with { StartTimeOfDay = "10:00", EndTimeOfDay = "09:00" };
        var result = _sut.TestValidate(command);
        result.ShouldHaveAnyValidationError();
    }

    [Fact]
    public void EndTime_Equal_StartTime_ShouldFail()
    {
        var command = ValidCommand() with { StartTimeOfDay = "10:00", EndTimeOfDay = "10:00" };
        var result = _sut.TestValidate(command);
        result.ShouldHaveAnyValidationError();
    }

    [Fact]
    public void EndsAt_Before_StartsFrom_ShouldFail()
    {
        var now = DateTimeOffset.UtcNow;
        var command = ValidCommand() with { StartsFrom = now, EndsAt = now.AddDays(-1) };
        var result = _sut.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.EndsAt);
    }

    [Fact]
    public void Null_EndsAt_ShouldPass()
    {
        var command = ValidCommand() with { EndsAt = null };
        var result = _sut.TestValidate(command);
        result.ShouldNotHaveValidationErrorFor(x => x.EndsAt);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Invalid_MaxOccurrences_ShouldFail(int maxOccurrences)
    {
        var command = ValidCommand() with { MaxOccurrences = maxOccurrences };
        var result = _sut.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.MaxOccurrences);
    }

    [Fact]
    public void Null_MaxOccurrences_ShouldPass()
    {
        var command = ValidCommand() with { MaxOccurrences = null };
        var result = _sut.TestValidate(command);
        result.ShouldNotHaveValidationErrorFor(x => x.MaxOccurrences);
    }

    [Fact]
    public void Valid_MaxOccurrences_ShouldPass()
    {
        var command = ValidCommand() with { MaxOccurrences = 10 };
        var result = _sut.TestValidate(command);
        result.ShouldNotHaveValidationErrorFor(x => x.MaxOccurrences);
    }

    [Theory]
    [InlineData("red")]
    [InlineData("#GGG")]
    [InlineData("123456")]
    public void Invalid_Color_Format_ShouldFail(string color)
    {
        var command = ValidCommand() with { Color = color };
        var result = _sut.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Color);
    }

    [Theory]
    [InlineData("#3B82F6")]
    [InlineData("#ffffff")]
    public void Valid_Color_Format_ShouldPass(string color)
    {
        var command = ValidCommand() with { Color = color };
        var result = _sut.TestValidate(command);
        result.ShouldNotHaveValidationErrorFor(x => x.Color);
    }

    [Fact]
    public void Null_Color_ShouldPass()
    {
        var command = ValidCommand() with { Color = null };
        var result = _sut.TestValidate(command);
        result.ShouldNotHaveValidationErrorFor(x => x.Color);
    }

    [Fact]
    public void PatientName_Exceeding_255_Chars_ShouldFail()
    {
        var command = ValidCommand() with { PatientName = new string('a', 256) };
        var result = _sut.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.PatientName);
    }

    [Fact]
    public void PatientPhone_Exceeding_50_Chars_ShouldFail()
    {
        var command = ValidCommand() with { PatientPhone = new string('1', 51) };
        var result = _sut.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.PatientPhone);
    }
}
