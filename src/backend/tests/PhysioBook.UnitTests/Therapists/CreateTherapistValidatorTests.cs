using FluentValidation.TestHelper;
using PhysioBook.Application.Therapists.Commands;
using PhysioBook.Application.Therapists.Validators;

namespace PhysioBook.UnitTests.Therapists;

public class CreateTherapistValidatorTests
{
    private readonly CreateTherapistValidator _sut = new();

    private static CreateTherapistCommand ValidCommand() => new(
        Email: "therapist@test.com",
        FirstName: "John",
        LastName: "Doe",
        Password: "Password1",
        Phone: null,
        Specialization: null,
        Color: null);

    [Fact]
    public void Valid_Command_ShouldPass()
    {
        var result = _sut.TestValidate(ValidCommand());
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Empty_Email_ShouldFail()
    {
        var command = ValidCommand() with { Email = "" };
        var result = _sut.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public void Invalid_Email_ShouldFail()
    {
        var command = ValidCommand() with { Email = "not-an-email" };
        var result = _sut.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public void Empty_FirstName_ShouldFail()
    {
        var command = ValidCommand() with { FirstName = "" };
        var result = _sut.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.FirstName);
    }

    [Fact]
    public void FirstName_Exceeding_100_Chars_ShouldFail()
    {
        var command = ValidCommand() with { FirstName = new string('a', 101) };
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
    public void LastName_Exceeding_100_Chars_ShouldFail()
    {
        var command = ValidCommand() with { LastName = new string('a', 101) };
        var result = _sut.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.LastName);
    }

    [Fact]
    public void Empty_Password_ShouldFail()
    {
        var command = ValidCommand() with { Password = "" };
        var result = _sut.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Password);
    }

    [Fact]
    public void Short_Password_ShouldFail()
    {
        var command = ValidCommand() with { Password = "Pass1" };
        var result = _sut.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Password);
    }

    [Fact]
    public void Password_Without_Uppercase_ShouldFail()
    {
        var command = ValidCommand() with { Password = "password1" };
        var result = _sut.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Password);
    }

    [Fact]
    public void Password_Without_Lowercase_ShouldFail()
    {
        var command = ValidCommand() with { Password = "PASSWORD1" };
        var result = _sut.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Password);
    }

    [Fact]
    public void Password_Without_Number_ShouldFail()
    {
        var command = ValidCommand() with { Password = "Password" };
        var result = _sut.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Password);
    }

    [Fact]
    public void Valid_Color_ShouldPass()
    {
        var command = ValidCommand() with { Color = "#FF5733" };
        var result = _sut.TestValidate(command);
        result.ShouldNotHaveValidationErrorFor(x => x.Color);
    }

    [Theory]
    [InlineData("red")]
    [InlineData("#GGG")]
    [InlineData("FF5733")]
    [InlineData("#FF573")]
    public void Invalid_Color_ShouldFail(string color)
    {
        var command = ValidCommand() with { Color = color };
        var result = _sut.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Color);
    }

    [Fact]
    public void Null_Color_ShouldPass()
    {
        var command = ValidCommand() with { Color = null };
        var result = _sut.TestValidate(command);
        result.ShouldNotHaveValidationErrorFor(x => x.Color);
    }
}
