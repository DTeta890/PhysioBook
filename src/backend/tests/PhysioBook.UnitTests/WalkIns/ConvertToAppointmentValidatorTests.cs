using FluentValidation.TestHelper;
using PhysioBook.Application.WalkIns.Commands;
using PhysioBook.Application.WalkIns.Validators;

namespace PhysioBook.UnitTests.WalkIns;

public class ConvertToAppointmentValidatorTests
{
    private readonly ConvertToAppointmentValidator _sut = new();

    private static ConvertToAppointmentCommand ValidCommand() => new(
        WalkInId: Guid.NewGuid(),
        TherapistId: Guid.NewGuid(),
        TreatmentTypeId: Guid.NewGuid(),
        StartTime: DateTimeOffset.UtcNow.AddHours(1),
        EndTime: DateTimeOffset.UtcNow.AddHours(2),
        Notes: null);

    [Fact]
    public void Valid_Command_ShouldPass()
    {
        var result = _sut.TestValidate(ValidCommand());
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Empty_WalkInId_ShouldFail()
    {
        var command = ValidCommand() with { WalkInId = Guid.Empty };
        var result = _sut.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.WalkInId);
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

    [Fact]
    public void EndTime_Before_StartTime_ShouldFail()
    {
        var now = DateTimeOffset.UtcNow;
        var command = ValidCommand() with { StartTime = now.AddHours(2), EndTime = now.AddHours(1) };
        var result = _sut.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.EndTime);
    }

    [Fact]
    public void EndTime_Equal_StartTime_ShouldFail()
    {
        var now = DateTimeOffset.UtcNow;
        var command = ValidCommand() with { StartTime = now, EndTime = now };
        var result = _sut.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.EndTime);
    }

    [Fact]
    public void Default_StartTime_ShouldFail()
    {
        var command = ValidCommand() with { StartTime = default };
        var result = _sut.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.StartTime);
    }

    [Fact]
    public void Default_EndTime_ShouldFail()
    {
        var command = ValidCommand() with { EndTime = default };
        var result = _sut.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.EndTime);
    }
}
