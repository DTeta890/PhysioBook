using FluentValidation.TestHelper;
using PhysioBook.Application.WalkIns.Commands;
using PhysioBook.Application.WalkIns.Validators;

namespace PhysioBook.UnitTests.WalkIns;

public class CallWalkInValidatorTests
{
    private readonly CallWalkInValidator _sut = new();

    private static CallWalkInCommand ValidCommand() => new(
        Id: Guid.NewGuid(),
        TherapistId: Guid.NewGuid());

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
    public void Empty_TherapistId_ShouldFail()
    {
        var command = ValidCommand() with { TherapistId = Guid.Empty };
        var result = _sut.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.TherapistId);
    }
}
