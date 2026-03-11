using FluentValidation.TestHelper;
using PhysioBook.Application.Appointments.Commands;
using PhysioBook.Application.Appointments.Validators;

namespace PhysioBook.UnitTests.Appointments;

public class UpdateAppointmentStatusValidatorTests
{
    private readonly UpdateAppointmentStatusValidator _sut = new();

    private static UpdateAppointmentStatusCommand ValidCommand() => new(
        Id: Guid.NewGuid(),
        Status: "confirmed",
        CancellationReason: null);

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
    public void Empty_Status_ShouldFail()
    {
        var command = ValidCommand() with { Status = "" };
        var result = _sut.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Status);
    }

    [Fact]
    public void Invalid_Status_ShouldFail()
    {
        var command = ValidCommand() with { Status = "invalid_status" };
        var result = _sut.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Status);
    }

    [Theory]
    [InlineData("scheduled")]
    [InlineData("confirmed")]
    [InlineData("in_progress")]
    [InlineData("completed")]
    [InlineData("cancelled")]
    [InlineData("no_show")]
    public void Valid_Status_Values_ShouldPass(string status)
    {
        var command = ValidCommand() with
        {
            Status = status,
            CancellationReason = status == "cancelled" ? "Patient requested" : null
        };
        var result = _sut.TestValidate(command);
        result.ShouldNotHaveValidationErrorFor(x => x.Status);
    }

    [Fact]
    public void Cancelled_Without_Reason_ShouldFail()
    {
        var command = ValidCommand() with { Status = "cancelled", CancellationReason = null };
        var result = _sut.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.CancellationReason);
    }

    [Fact]
    public void Cancelled_With_Empty_Reason_ShouldFail()
    {
        var command = ValidCommand() with { Status = "cancelled", CancellationReason = "" };
        var result = _sut.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.CancellationReason);
    }

    [Fact]
    public void Cancelled_With_Reason_ShouldPass()
    {
        var command = ValidCommand() with { Status = "cancelled", CancellationReason = "Patient requested cancellation" };
        var result = _sut.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void NonCancelled_Without_Reason_ShouldPass()
    {
        var command = ValidCommand() with { Status = "completed", CancellationReason = null };
        var result = _sut.TestValidate(command);
        result.ShouldNotHaveValidationErrorFor(x => x.CancellationReason);
    }
}
