using FluentValidation.TestHelper;
using PhysioBook.Application.Auth.Commands;
using PhysioBook.Application.Auth.Validators;

namespace PhysioBook.UnitTests.Auth;

public class RegisterValidatorTests
{
    private readonly RegisterValidator _sut = new();

    [Fact]
    public void Valid_Registration_ShouldPass()
    {
        var command = new RegisterCommand(
            "user@test.com", "Password1", "John", "Doe",
            null, "therapist", true, null, null);
        var result = _sut.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Short_Password_ShouldFail()
    {
        var command = new RegisterCommand(
            "user@test.com", "Pass1", "John", "Doe",
            null, "therapist", true, null, null);
        var result = _sut.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Password);
    }

    [Fact]
    public void Password_Without_Uppercase_ShouldFail()
    {
        var command = new RegisterCommand(
            "user@test.com", "password1", "John", "Doe",
            null, "therapist", true, null, null);
        var result = _sut.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Password);
    }

    [Fact]
    public void Password_Without_Number_ShouldFail()
    {
        var command = new RegisterCommand(
            "user@test.com", "Password", "John", "Doe",
            null, "therapist", true, null, null);
        var result = _sut.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Password);
    }

    [Fact]
    public void Empty_FirstName_ShouldFail()
    {
        var command = new RegisterCommand(
            "user@test.com", "Password1", "", "Doe",
            null, "therapist", true, null, null);
        var result = _sut.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.FirstName);
    }

    [Fact]
    public void Empty_Role_ShouldFail()
    {
        var command = new RegisterCommand(
            "user@test.com", "Password1", "John", "Doe",
            null, "", true, null, null);
        var result = _sut.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Role);
    }
}
