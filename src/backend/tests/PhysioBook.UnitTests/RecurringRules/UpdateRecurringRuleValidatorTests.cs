using FluentValidation.TestHelper;
using PhysioBook.Application.RecurringRules.Commands;
using PhysioBook.Application.RecurringRules.Validators;

namespace PhysioBook.UnitTests.RecurringRules;

public class UpdateRecurringRuleValidatorTests
{
    private readonly UpdateRecurringRuleValidator _sut = new();

    private static UpdateRecurringRuleCommand ValidCommand() => new(
        Id: Guid.NewGuid(),
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
        Color: "#3B82F6",
        IsActive: true);

    [Fact]
    public void Valid_Command_ShouldPass()
    {
        var result = _sut.TestValidate(ValidCommand());
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Empty_Id_ShouldFail()
    {
        var command = ValidCommand() with { Id = Guid.Empty };
        var result = _sut.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public void Empty_TherapistId_ShouldFail()
    {
        var command = ValidCommand() with { TherapistId = Guid.Empty };
        var result = _sut.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.TherapistId);
    }

    [Theory]
    [InlineData("daily")]
    [InlineData("")]
    public void Invalid_Frequency_ShouldFail(string frequency)
    {
        var command = ValidCommand() with { Frequency = frequency };
        var result = _sut.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Frequency);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(7)]
    public void Invalid_DayOfWeek_ShouldFail(int dayOfWeek)
    {
        var command = ValidCommand() with { DayOfWeek = dayOfWeek };
        var result = _sut.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.DayOfWeek);
    }

    [Fact]
    public void EndTime_Before_StartTime_ShouldFail()
    {
        var command = ValidCommand() with { StartTimeOfDay = "10:00", EndTimeOfDay = "09:00" };
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

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Invalid_MaxOccurrences_ShouldFail(int maxOccurrences)
    {
        var command = ValidCommand() with { MaxOccurrences = maxOccurrences };
        var result = _sut.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.MaxOccurrences);
    }
}
