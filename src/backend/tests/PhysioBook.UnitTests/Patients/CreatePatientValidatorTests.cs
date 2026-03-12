using FluentValidation.TestHelper;
using PhysioBook.Application.Patients.Commands;
using PhysioBook.Application.Patients.Validators;

namespace PhysioBook.UnitTests.Patients;

public class CreatePatientValidatorTests
{
    private readonly CreatePatientValidator _sut = new();

    private static CreatePatientCommand ValidCommand() => new(
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
        Notes: null);

    [Fact]
    public void Valid_Command_ShouldPass()
    {
        var result = _sut.TestValidate(ValidCommand());
        result.ShouldNotHaveAnyValidationErrors();
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
        var command = ValidCommand() with { Email = "not-an-email" };
        var result = _sut.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public void Invalid_Gender_ShouldFail()
    {
        var command = ValidCommand() with { Gender = "invalid" };
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

    [Fact]
    public void LastName_Too_Long_ShouldFail()
    {
        var command = ValidCommand() with { LastName = new string('a', 101) };
        var result = _sut.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.LastName);
    }

    [Fact]
    public void Email_Too_Long_ShouldFail()
    {
        var command = ValidCommand() with { Email = new string('a', 245) + "@example.com" };
        var result = _sut.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public void Phone_Too_Long_ShouldFail()
    {
        var command = ValidCommand() with { Phone = new string('1', 51) };
        var result = _sut.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Phone);
    }

    [Fact]
    public void Null_Email_ShouldPass()
    {
        var command = ValidCommand() with { Email = null };
        var result = _sut.TestValidate(command);
        result.ShouldNotHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public void Null_Gender_ShouldPass()
    {
        var command = ValidCommand() with { Gender = null };
        var result = _sut.TestValidate(command);
        result.ShouldNotHaveValidationErrorFor(x => x.Gender);
    }

    [Theory]
    [InlineData("male")]
    [InlineData("female")]
    [InlineData("other")]
    public void Valid_Gender_Values_ShouldPass(string gender)
    {
        var command = ValidCommand() with { Gender = gender };
        var result = _sut.TestValidate(command);
        result.ShouldNotHaveValidationErrorFor(x => x.Gender);
    }

    [Fact]
    public void EmergencyContactName_Too_Long_ShouldFail()
    {
        var command = ValidCommand() with { EmergencyContactName = new string('a', 201) };
        var result = _sut.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.EmergencyContactName);
    }
}
