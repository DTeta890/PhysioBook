using FluentValidation.TestHelper;
using PhysioBook.Application.PatientDocuments.Commands;
using PhysioBook.Application.PatientDocuments.Validators;

namespace PhysioBook.UnitTests.PatientDocuments;

public class DeleteDocumentValidatorTests
{
    private readonly DeleteDocumentValidator _sut = new();

    [Fact]
    public void Valid_Id_ShouldPass()
    {
        var command = new DeleteDocumentCommand(Guid.NewGuid());
        var result = _sut.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Empty_Id_ShouldFail()
    {
        var command = new DeleteDocumentCommand(Guid.Empty);
        var result = _sut.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Id);
    }
}
