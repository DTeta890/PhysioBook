using FluentValidation.TestHelper;
using PhysioBook.Application.TreatmentNotes.Commands;
using PhysioBook.Application.TreatmentNotes.Validators;

namespace PhysioBook.UnitTests.TreatmentNotes;

public class CreateTreatmentNoteValidatorTests
{
    private readonly CreateTreatmentNoteValidator _sut = new();

    private static CreateTreatmentNoteCommand ValidCommand() => new(
        AppointmentId: Guid.NewGuid(),
        Subjective: "Patient reports pain in lower back",
        Objective: "Limited range of motion observed",
        Assessment: "Lumbar strain, improving",
        Plan: "Continue exercises, follow up in 1 week",
        Diagnosis: "Lumbar strain",
        TreatmentProvided: "Manual therapy, stretching",
        PainLevelBefore: 7,
        PainLevelAfter: 4,
        RangeOfMotionNotes: "Flexion 60 degrees",
        ExercisesPrescribed: "Core strengthening",
        FollowUpInstructions: "Return in 1 week");

    [Fact]
    public void Valid_Command_ShouldPass()
    {
        var result = _sut.TestValidate(ValidCommand());
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Empty_AppointmentId_ShouldFail()
    {
        var command = ValidCommand() with { AppointmentId = Guid.Empty };
        var result = _sut.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.AppointmentId);
    }

    [Fact]
    public void PainLevelBefore_Below_Zero_ShouldFail()
    {
        var command = ValidCommand() with { PainLevelBefore = -1 };
        var result = _sut.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.PainLevelBefore);
    }

    [Fact]
    public void PainLevelBefore_Above_Ten_ShouldFail()
    {
        var command = ValidCommand() with { PainLevelBefore = 11 };
        var result = _sut.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.PainLevelBefore);
    }

    [Fact]
    public void PainLevelAfter_Below_Zero_ShouldFail()
    {
        var command = ValidCommand() with { PainLevelAfter = -1 };
        var result = _sut.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.PainLevelAfter);
    }

    [Fact]
    public void PainLevelAfter_Above_Ten_ShouldFail()
    {
        var command = ValidCommand() with { PainLevelAfter = 11 };
        var result = _sut.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.PainLevelAfter);
    }

    [Fact]
    public void No_Soap_Fields_ShouldFail()
    {
        var command = ValidCommand() with
        {
            Subjective = null,
            Objective = null,
            Assessment = null,
            Plan = null
        };
        var result = _sut.TestValidate(command);
        result.ShouldHaveAnyValidationError()
            .WithErrorMessage("At least one SOAP field (Subjective, Objective, Assessment, or Plan) must be provided.");
    }

    [Fact]
    public void Only_Subjective_Provided_ShouldPass()
    {
        var command = ValidCommand() with
        {
            Subjective = "Patient reports pain",
            Objective = null,
            Assessment = null,
            Plan = null
        };
        var result = _sut.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Null_PainLevels_ShouldPass()
    {
        var command = ValidCommand() with
        {
            PainLevelBefore = null,
            PainLevelAfter = null
        };
        var result = _sut.TestValidate(command);
        result.ShouldNotHaveValidationErrorFor(x => x.PainLevelBefore);
        result.ShouldNotHaveValidationErrorFor(x => x.PainLevelAfter);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(5)]
    [InlineData(10)]
    public void Valid_PainLevelBefore_ShouldPass(int painLevel)
    {
        var command = ValidCommand() with { PainLevelBefore = painLevel };
        var result = _sut.TestValidate(command);
        result.ShouldNotHaveValidationErrorFor(x => x.PainLevelBefore);
    }
}
