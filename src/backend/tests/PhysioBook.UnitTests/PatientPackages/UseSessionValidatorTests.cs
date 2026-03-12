using FluentValidation.TestHelper;
using PhysioBook.Application.PatientPackages.Commands;
using PhysioBook.Application.PatientPackages.Validators;

namespace PhysioBook.UnitTests.PatientPackages;

public class UseSessionValidatorTests
{
    private readonly UseSessionValidator _sut = new();

    [Fact]
    public void Valid_Command_ShouldPass()
    {
        var command = new UseSessionCommand(Guid.NewGuid());
        var result = _sut.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Empty_Id_ShouldFail()
    {
        var command = new UseSessionCommand(Guid.Empty);
        var result = _sut.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Id);
    }
}
