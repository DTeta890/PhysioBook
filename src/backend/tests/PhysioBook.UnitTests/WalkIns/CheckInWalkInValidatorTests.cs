using FluentValidation.TestHelper;
using PhysioBook.Application.WalkIns.Commands;
using PhysioBook.Application.WalkIns.Validators;

namespace PhysioBook.UnitTests.WalkIns;

public class CheckInWalkInValidatorTests
{
    private readonly CheckInWalkInValidator _sut = new();

    private static CheckInWalkInCommand ValidCommand() => new(
        PatientId: null,
        PatientName: "John Doe",
        PatientPhone: "+355691234567",
        TreatmentTypeId: Guid.NewGuid(),
        ReasonForVisit: "Back pain",
        Priority: 1,
        Notes: null);

    [Fact]
    public void Valid_Command_ShouldPass()
    {
        var result = _sut.TestValidate(ValidCommand());
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Empty_PatientName_ShouldFail()
    {
        var command = ValidCommand() with { PatientName = "" };
        var result = _sut.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.PatientName);
    }

    [Fact]
    public void Null_PatientName_ShouldFail()
    {
        var command = ValidCommand() with { PatientName = null! };
        var result = _sut.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.PatientName);
    }

    [Fact]
    public void PatientName_Exceeding_200_Chars_ShouldFail()
    {
        var command = ValidCommand() with { PatientName = new string('a', 201) };
        var result = _sut.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.PatientName);
    }

    [Fact]
    public void PatientName_At_200_Chars_ShouldPass()
    {
        var command = ValidCommand() with { PatientName = new string('a', 200) };
        var result = _sut.TestValidate(command);
        result.ShouldNotHaveValidationErrorFor(x => x.PatientName);
    }

    [Fact]
    public void PatientPhone_Exceeding_50_Chars_ShouldFail()
    {
        var command = ValidCommand() with { PatientPhone = new string('1', 51) };
        var result = _sut.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.PatientPhone);
    }

    [Fact]
    public void Null_PatientPhone_ShouldPass()
    {
        var command = ValidCommand() with { PatientPhone = null };
        var result = _sut.TestValidate(command);
        result.ShouldNotHaveValidationErrorFor(x => x.PatientPhone);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    public void Valid_Priority_ShouldPass(int priority)
    {
        var command = ValidCommand() with { Priority = priority };
        var result = _sut.TestValidate(command);
        result.ShouldNotHaveValidationErrorFor(x => x.Priority);
    }

    [Fact]
    public void Null_Priority_ShouldPass()
    {
        var command = ValidCommand() with { Priority = null };
        var result = _sut.TestValidate(command);
        result.ShouldNotHaveValidationErrorFor(x => x.Priority);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(3)]
    [InlineData(-1)]
    public void Invalid_Priority_ShouldFail(int priority)
    {
        var command = ValidCommand() with { Priority = priority };
        var result = _sut.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Priority);
    }

    [Fact]
    public void ReasonForVisit_Exceeding_500_Chars_ShouldFail()
    {
        var command = ValidCommand() with { ReasonForVisit = new string('a', 501) };
        var result = _sut.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.ReasonForVisit);
    }

    [Fact]
    public void Notes_Exceeding_1000_Chars_ShouldFail()
    {
        var command = ValidCommand() with { Notes = new string('a', 1001) };
        var result = _sut.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Notes);
    }

    [Fact]
    public void Null_Notes_ShouldPass()
    {
        var command = ValidCommand() with { Notes = null };
        var result = _sut.TestValidate(command);
        result.ShouldNotHaveValidationErrorFor(x => x.Notes);
    }

    [Fact]
    public void With_PatientId_ShouldPass()
    {
        var command = ValidCommand() with { PatientId = Guid.NewGuid() };
        var result = _sut.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
