using FluentValidation.TestHelper;
using PhysioBook.Application.Therapists.Commands;
using PhysioBook.Application.Therapists.Validators;

namespace PhysioBook.UnitTests.Therapists;

public class UpdateTherapistValidatorTests
{
    private readonly UpdateTherapistValidator _sut = new();

    private static UpdateTherapistCommand ValidCommand() => new(
        Id: Guid.NewGuid(),
        FirstName: "John",
        LastName: "Doe",
        Phone: null,
        Specialization: null,
        Color: null,
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
    public void Valid_Color_ShouldPass()
    {
        var command = ValidCommand() with { Color = "#AABBCC" };
        var result = _sut.TestValidate(command);
        result.ShouldNotHaveValidationErrorFor(x => x.Color);
    }

    [Theory]
    [InlineData("blue")]
    [InlineData("#ZZZ")]
    [InlineData("AABBCC")]
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
