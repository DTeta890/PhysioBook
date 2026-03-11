using FluentValidation.TestHelper;
using PhysioBook.Application.TreatmentTypes.Commands;
using PhysioBook.Application.TreatmentTypes.Validators;

namespace PhysioBook.UnitTests.TreatmentTypes;

public class UpdateTreatmentTypeValidatorTests
{
    private readonly UpdateTreatmentTypeValidator _sut = new();

    private static UpdateTreatmentTypeCommand ValidCommand() => new(
        Id: Guid.NewGuid(),
        Name: "Physiotherapy Session",
        Description: "Standard physio session",
        DurationMinutes: 30,
        Price: 25.00m,
        Color: "#3B82F6",
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

    [Fact]
    public void Negative_Price_ShouldFail()
    {
        var command = ValidCommand() with { Price = -0.01m };
        var result = _sut.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Price);
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

    [Fact]
    public void Null_Color_ShouldPass()
    {
        var command = ValidCommand() with { Color = null };
        var result = _sut.TestValidate(command);
        result.ShouldNotHaveValidationErrorFor(x => x.Color);
    }

    [Fact]
    public void IsActive_False_ShouldPass()
    {
        var command = ValidCommand() with { IsActive = false };
        var result = _sut.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
