using FluentValidation.TestHelper;
using PhysioBook.Application.Appointments.Commands;
using PhysioBook.Application.Appointments.Validators;

namespace PhysioBook.UnitTests.Appointments;

public class UpdateAppointmentValidatorTests
{
    private readonly UpdateAppointmentValidator _sut = new();

    private static UpdateAppointmentCommand ValidCommand() => new(
        Id: Guid.NewGuid(),
        TherapistId: Guid.NewGuid(),
        PatientId: null,
        PatientName: "John Doe",
        PatientPhone: "+355691234567",
        TreatmentTypeId: Guid.NewGuid(),
        StartTime: DateTimeOffset.UtcNow.AddHours(1),
        EndTime: DateTimeOffset.UtcNow.AddHours(2),
        Notes: "Updated notes",
        Color: null);

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
    public void Invalid_Color_Format_ShouldFail(string color)
    {
        var command = ValidCommand() with { Color = color };
        var result = _sut.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Color);
    }

    [Fact]
    public void Valid_Color_ShouldPass()
    {
        var command = ValidCommand() with { Color = "#3B82F6" };
        var result = _sut.TestValidate(command);
        result.ShouldNotHaveValidationErrorFor(x => x.Color);
    }
}
