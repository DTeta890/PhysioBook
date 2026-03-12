using FluentValidation.TestHelper;
using PhysioBook.Application.TreatmentPackages.Commands;
using PhysioBook.Application.TreatmentPackages.Validators;

namespace PhysioBook.UnitTests.TreatmentPackages;

public class CreateTreatmentPackageValidatorTests
{
    private readonly CreateTreatmentPackageValidator _sut = new();

    private static CreateTreatmentPackageCommand ValidCommand() => new(
        Name: "10 Sessions Physiotherapy",
        TreatmentTypeId: Guid.NewGuid(),
        TotalSessions: 10,
        Price: 200m,
        ValidityDays: 90,
        Description: "Standard physiotherapy package");

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
    public void Name_Exceeding_200_Chars_ShouldFail()
    {
        var command = ValidCommand() with { Name = new string('a', 201) };
        var result = _sut.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Name_At_200_Chars_ShouldPass()
    {
        var command = ValidCommand() with { Name = new string('a', 200) };
        var result = _sut.TestValidate(command);
        result.ShouldNotHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Empty_TreatmentTypeId_ShouldFail()
    {
        var command = ValidCommand() with { TreatmentTypeId = Guid.Empty };
        var result = _sut.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.TreatmentTypeId);
    }

    [Fact]
    public void TotalSessions_Zero_ShouldFail()
    {
        var command = ValidCommand() with { TotalSessions = 0 };
        var result = _sut.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.TotalSessions);
    }

    [Fact]
    public void TotalSessions_Negative_ShouldFail()
    {
        var command = ValidCommand() with { TotalSessions = -1 };
        var result = _sut.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.TotalSessions);
    }

    [Fact]
    public void TotalSessions_Positive_ShouldPass()
    {
        var command = ValidCommand() with { TotalSessions = 1 };
        var result = _sut.TestValidate(command);
        result.ShouldNotHaveValidationErrorFor(x => x.TotalSessions);
    }

    [Fact]
    public void Price_Negative_ShouldFail()
    {
        var command = ValidCommand() with { Price = -1m };
        var result = _sut.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Price);
    }

    [Fact]
    public void Price_Zero_ShouldPass()
    {
        var command = ValidCommand() with { Price = 0m };
        var result = _sut.TestValidate(command);
        result.ShouldNotHaveValidationErrorFor(x => x.Price);
    }

    [Fact]
    public void ValidityDays_Zero_ShouldFail()
    {
        var command = ValidCommand() with { ValidityDays = 0 };
        var result = _sut.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.ValidityDays);
    }

    [Fact]
    public void ValidityDays_Negative_ShouldFail()
    {
        var command = ValidCommand() with { ValidityDays = -5 };
        var result = _sut.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.ValidityDays);
    }

    [Fact]
    public void ValidityDays_Null_ShouldPass()
    {
        var command = ValidCommand() with { ValidityDays = null };
        var result = _sut.TestValidate(command);
        result.ShouldNotHaveValidationErrorFor(x => x.ValidityDays);
    }

    [Fact]
    public void ValidityDays_Positive_ShouldPass()
    {
        var command = ValidCommand() with { ValidityDays = 30 };
        var result = _sut.TestValidate(command);
        result.ShouldNotHaveValidationErrorFor(x => x.ValidityDays);
    }

    [Fact]
    public void Description_Exceeding_1000_Chars_ShouldFail()
    {
        var command = ValidCommand() with { Description = new string('a', 1001) };
        var result = _sut.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Description);
    }

    [Fact]
    public void Description_Null_ShouldPass()
    {
        var command = ValidCommand() with { Description = null };
        var result = _sut.TestValidate(command);
        result.ShouldNotHaveValidationErrorFor(x => x.Description);
    }
}
