using FluentValidation.TestHelper;
using PhysioBook.Application.TreatmentPackages.Commands;
using PhysioBook.Application.TreatmentPackages.Validators;

namespace PhysioBook.UnitTests.TreatmentPackages;

public class UpdateTreatmentPackageValidatorTests
{
    private readonly UpdateTreatmentPackageValidator _sut = new();

    private static UpdateTreatmentPackageCommand ValidCommand() => new(
        Id: Guid.NewGuid(),
        Name: "10 Sessions Physiotherapy",
        TreatmentTypeId: Guid.NewGuid(),
        TotalSessions: 10,
        Price: 200m,
        ValidityDays: 90,
        Description: "Standard physiotherapy package",
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
    public void Name_Exceeding_200_Chars_ShouldFail()
    {
        var command = ValidCommand() with { Name = new string('a', 201) };
        var result = _sut.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Name);
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
    public void Price_Negative_ShouldFail()
    {
        var command = ValidCommand() with { Price = -1m };
        var result = _sut.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Price);
    }

    [Fact]
    public void ValidityDays_Zero_ShouldFail()
    {
        var command = ValidCommand() with { ValidityDays = 0 };
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
    public void Description_Exceeding_1000_Chars_ShouldFail()
    {
        var command = ValidCommand() with { Description = new string('a', 1001) };
        var result = _sut.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Description);
    }
}
