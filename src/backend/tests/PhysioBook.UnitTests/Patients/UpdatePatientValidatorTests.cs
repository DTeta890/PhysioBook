using FluentValidation.TestHelper;
using PhysioBook.Application.Patients.Commands;
using PhysioBook.Application.Patients.Validators;

namespace PhysioBook.UnitTests.Patients;

public class UpdatePatientValidatorTests
{
    private readonly UpdatePatientValidator _sut = new();

    private static UpdatePatientCommand ValidCommand() => new(
        Id: Guid.NewGuid(),
        FirstName: "Arian",
        LastName: "Hoxha",
        Email: "arian@example.com",
        Phone: "+355691234567",
        DateOfBirth: new DateOnly(1990, 5, 15),
        Gender: "male",
        Address: "Rruga Elbasan 123",
        City: "Elbasan",
        EmergencyContactName: "Elira Hoxha",
        EmergencyContactPhone: "+355691234568",
        MedicalHistory: null,
        Allergies: null,
        Notes: null,
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
    public void Empty_FirstName_ShouldFail()
    {
        var command = ValidCommand() with { FirstName = "" };
        var result = _sut.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.FirstName);
    }

    [Fact]
    public void Empty_LastName_ShouldFail()
    {
        var command = ValidCommand() with { LastName = "" };
        var result = _sut.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.LastName);
    }

    [Fact]
    public void Invalid_Email_ShouldFail()
    {
        var command = ValidCommand() with { Email = "bad-email" };
        var result = _sut.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public void Invalid_Gender_ShouldFail()
    {
        var command = ValidCommand() with { Gender = "unknown" };
        var result = _sut.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Gender);
    }

    [Fact]
    public void FirstName_Too_Long_ShouldFail()
    {
        var command = ValidCommand() with { FirstName = new string('a', 101) };
        var result = _sut.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.FirstName);
    }
}
