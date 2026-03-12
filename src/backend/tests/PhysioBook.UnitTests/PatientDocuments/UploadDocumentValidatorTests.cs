using FluentValidation.TestHelper;
using PhysioBook.Application.PatientDocuments.Commands;
using PhysioBook.Application.PatientDocuments.Validators;

namespace PhysioBook.UnitTests.PatientDocuments;

public class UploadDocumentValidatorTests
{
    private readonly UploadDocumentValidator _sut = new();

    private static UploadDocumentCommand ValidCommand() => new(
        PatientId: Guid.NewGuid(),
        FileName: "xray_shoulder.jpg",
        ContentType: "image/jpeg",
        FileSizeBytes: 1024 * 1024, // 1 MB
        Category: "xray",
        Description: "Left shoulder X-ray",
        FileStream: Stream.Null);

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
    public void Empty_FileName_ShouldFail()
    {
        var command = ValidCommand() with { FileName = string.Empty };
        var result = _sut.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.FileName);
    }

    [Fact]
    public void FileName_Exceeding_500_Chars_ShouldFail()
    {
        var command = ValidCommand() with { FileName = new string('a', 501) };
        var result = _sut.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.FileName);
    }

    [Fact]
    public void FileName_At_500_Chars_ShouldPass()
    {
        var command = ValidCommand() with { FileName = new string('a', 500) };
        var result = _sut.TestValidate(command);
        result.ShouldNotHaveValidationErrorFor(x => x.FileName);
    }

    [Fact]
    public void Empty_ContentType_ShouldFail()
    {
        var command = ValidCommand() with { ContentType = string.Empty };
        var result = _sut.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.ContentType);
    }

    [Fact]
    public void FileSizeBytes_Zero_ShouldFail()
    {
        var command = ValidCommand() with { FileSizeBytes = 0 };
        var result = _sut.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.FileSizeBytes);
    }

    [Fact]
    public void FileSizeBytes_Negative_ShouldFail()
    {
        var command = ValidCommand() with { FileSizeBytes = -1 };
        var result = _sut.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.FileSizeBytes);
    }

    [Fact]
    public void FileSizeBytes_Exceeding_50MB_ShouldFail()
    {
        var command = ValidCommand() with { FileSizeBytes = 50L * 1024 * 1024 + 1 };
        var result = _sut.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.FileSizeBytes);
    }

    [Fact]
    public void FileSizeBytes_Exactly_50MB_ShouldPass()
    {
        var command = ValidCommand() with { FileSizeBytes = 50L * 1024 * 1024 };
        var result = _sut.TestValidate(command);
        result.ShouldNotHaveValidationErrorFor(x => x.FileSizeBytes);
    }

    [Fact]
    public void Empty_Category_ShouldFail()
    {
        var command = ValidCommand() with { Category = string.Empty };
        var result = _sut.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Category);
    }

    [Theory]
    [InlineData("xray")]
    [InlineData("mri")]
    [InlineData("referral")]
    [InlineData("consent")]
    [InlineData("insurance")]
    [InlineData("other")]
    [InlineData("Xray")]
    [InlineData("MRI")]
    public void Valid_Category_ShouldPass(string category)
    {
        var command = ValidCommand() with { Category = category };
        var result = _sut.TestValidate(command);
        result.ShouldNotHaveValidationErrorFor(x => x.Category);
    }

    [Theory]
    [InlineData("invalid")]
    [InlineData("photo")]
    [InlineData("lab_result")]
    public void Invalid_Category_ShouldFail(string category)
    {
        var command = ValidCommand() with { Category = category };
        var result = _sut.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Category);
    }

    [Fact]
    public void Null_Description_ShouldPass()
    {
        var command = ValidCommand() with { Description = null };
        var result = _sut.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
