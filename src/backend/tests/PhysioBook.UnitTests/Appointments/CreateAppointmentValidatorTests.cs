using FluentValidation.TestHelper;
using PhysioBook.Application.Appointments.Commands;
using PhysioBook.Application.Appointments.Validators;

namespace PhysioBook.UnitTests.Appointments;

public class CreateAppointmentValidatorTests
{
    private readonly CreateAppointmentValidator _sut = new();

    private static CreateAppointmentCommand ValidCommand() => new(
        TherapistId: Guid.NewGuid(),
        PatientId: null,
        PatientName: "John Doe",
        PatientPhone: "+355691234567",
        TreatmentTypeId: Guid.NewGuid(),
        StartTime: DateTimeOffset.UtcNow.AddHours(1),
        EndTime: DateTimeOffset.UtcNow.AddHours(2),
        Notes: "First session",
        IsWalkIn: false,
        Color: null);

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
    public void Null_PatientName_ShouldPass()
    {
        var command = ValidCommand() with { PatientName = null };
        var result = _sut.TestValidate(command);
        result.ShouldNotHaveValidationErrorFor(x => x.PatientName);
    }

    [Fact]
    public void Null_PatientPhone_ShouldPass()
    {
        var command = ValidCommand() with { PatientPhone = null };
        var result = _sut.TestValidate(command);
        result.ShouldNotHaveValidationErrorFor(x => x.PatientPhone);
    }
}
