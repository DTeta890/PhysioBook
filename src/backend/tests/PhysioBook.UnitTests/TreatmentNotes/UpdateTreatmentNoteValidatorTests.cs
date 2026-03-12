using FluentValidation.TestHelper;
using PhysioBook.Application.TreatmentNotes.Commands;
using PhysioBook.Application.TreatmentNotes.Validators;

namespace PhysioBook.UnitTests.TreatmentNotes;

public class UpdateTreatmentNoteValidatorTests
{
    private readonly UpdateTreatmentNoteValidator _sut = new();

    private static UpdateTreatmentNoteCommand ValidCommand() => new(
        Id: Guid.NewGuid(),
        Subjective: "Patient reports improvement",
        Objective: "Full range of motion restored",
        Assessment: "Resolved",
        Plan: "Discharge",
        Diagnosis: "Lumbar strain - resolved",
        TreatmentProvided: "Manual therapy",
        PainLevelBefore: 3,
        PainLevelAfter: 1,
        RangeOfMotionNotes: "Full flexion achieved",
        ExercisesPrescribed: "Maintenance exercises",
        FollowUpInstructions: "As needed");

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
    public void PainLevelBefore_Out_Of_Range_ShouldFail()
    {
        var command = ValidCommand() with { PainLevelBefore = 15 };
        var result = _sut.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.PainLevelBefore);
    }

    [Fact]
    public void PainLevelAfter_Out_Of_Range_ShouldFail()
    {
        var command = ValidCommand() with { PainLevelAfter = -5 };
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
    public void Only_Plan_Provided_ShouldPass()
    {
        var command = ValidCommand() with
        {
            Subjective = null,
            Objective = null,
            Assessment = null,
            Plan = "Continue treatment"
        };
        var result = _sut.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
