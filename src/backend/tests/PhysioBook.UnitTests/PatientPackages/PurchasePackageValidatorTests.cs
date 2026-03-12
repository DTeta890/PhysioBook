using FluentValidation.TestHelper;
using PhysioBook.Application.PatientPackages.Commands;
using PhysioBook.Application.PatientPackages.Validators;

namespace PhysioBook.UnitTests.PatientPackages;

public class PurchasePackageValidatorTests
{
    private readonly PurchasePackageValidator _sut = new();

    private static PurchasePackageCommand ValidCommand() => new(
        PatientId: Guid.NewGuid(),
        TreatmentPackageId: Guid.NewGuid(),
        Notes: "Patient purchased at front desk");

    [Fact]
    public void Valid_Command_ShouldPass()
    {
        var result = _sut.TestValidate(ValidCommand());
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Empty_PatientId_ShouldFail()
    {
        var command = ValidCommand() with { PatientId = Guid.Empty };
        var result = _sut.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.PatientId);
    }

    [Fact]
    public void Empty_TreatmentPackageId_ShouldFail()
    {
        var command = ValidCommand() with { TreatmentPackageId = Guid.Empty };
        var result = _sut.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.TreatmentPackageId);
    }

    [Fact]
    public void Notes_Exceeding_1000_Chars_ShouldFail()
    {
        var command = ValidCommand() with { Notes = new string('a', 1001) };
        var result = _sut.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Notes);
    }

    [Fact]
    public void Notes_Null_ShouldPass()
    {
        var command = ValidCommand() with { Notes = null };
        var result = _sut.TestValidate(command);
        result.ShouldNotHaveValidationErrorFor(x => x.Notes);
    }

    [Fact]
    public void Notes_At_1000_Chars_ShouldPass()
    {
        var command = ValidCommand() with { Notes = new string('a', 1000) };
        var result = _sut.TestValidate(command);
        result.ShouldNotHaveValidationErrorFor(x => x.Notes);
    }
}
