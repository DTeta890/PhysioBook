using FluentValidation.TestHelper;
using PhysioBook.Application.TreatmentTypes.Commands;
using PhysioBook.Application.TreatmentTypes.Validators;

namespace PhysioBook.UnitTests.TreatmentTypes;

public class CreateTreatmentTypeValidatorTests
{
    private readonly CreateTreatmentTypeValidator _sut = new();

    private static CreateTreatmentTypeCommand ValidCommand() => new(
        Name: "Physiotherapy Session",
        Description: "Standard physio session",
        DurationMinutes: 30,
        Price: 25.00m,
        Color: "#3B82F6");

    [Fact]
    public void Valid_Command_ShouldPass()
    {
        var result = _sut.TestValidate(ValidCommand());
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Empty_Name_ShouldFail()
    {
        var command = ValidCommand() with { Name = "" };
        var result = _sut.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Name_Exceeding_255_Chars_ShouldFail()
    {
        var command = ValidCommand() with { Name = new string('a', 256) };
        var result = _sut.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-100)]
    public void DurationMinutes_Zero_Or_Negative_ShouldFail(int duration)
    {
        var command = ValidCommand() with { DurationMinutes = duration };
        var result = _sut.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.DurationMinutes);
    }

    [Fact]
    public void DurationMinutes_Exceeding_480_ShouldFail()
    {
        var command = ValidCommand() with { DurationMinutes = 481 };
        var result = _sut.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.DurationMinutes);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(30)]
    [InlineData(60)]
    [InlineData(480)]
    public void Valid_DurationMinutes_ShouldPass(int duration)
    {
        var command = ValidCommand() with { DurationMinutes = duration };
        var result = _sut.TestValidate(command);
        result.ShouldNotHaveValidationErrorFor(x => x.DurationMinutes);
    }

    [Fact]
    public void Negative_Price_ShouldFail()
    {
        var command = ValidCommand() with { Price = -1m };
        var result = _sut.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Price);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(25.50)]
    [InlineData(100)]
    public void Valid_Price_ShouldPass(double price)
    {
        var command = ValidCommand() with { Price = (decimal)price };
        var result = _sut.TestValidate(command);
        result.ShouldNotHaveValidationErrorFor(x => x.Price);
    }

    [Theory]
    [InlineData("red")]
    [InlineData("#GGG")]
    [InlineData("123456")]
    [InlineData("#12345")]
    [InlineData("#1234567")]
    public void Invalid_Color_Format_ShouldFail(string color)
    {
        var command = ValidCommand() with { Color = color };
        var result = _sut.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Color);
    }

    [Theory]
    [InlineData("#3B82F6")]
    [InlineData("#ffffff")]
    [InlineData("#000000")]
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
    public void Null_Description_ShouldPass()
    {
        var command = ValidCommand() with { Description = null };
        var result = _sut.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
