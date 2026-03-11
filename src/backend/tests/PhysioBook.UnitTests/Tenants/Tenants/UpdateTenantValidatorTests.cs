using FluentValidation.TestHelper;
using PhysioBook.Application.Tenants.Commands;
using PhysioBook.Application.Tenants.Validators;

namespace PhysioBook.UnitTests.Tenants;

public class UpdateTenantValidatorTests
{
    private readonly UpdateTenantValidator _sut = new();

    private static UpdateTenantCommand ValidCommand() => new(
        TenantId: Guid.NewGuid(),
        Name: "Updated Clinic",
        ContactEmail: "updated@test.com",
        Phone: null,
        Address: null,
        City: null,
        Subdomain: null,
        IsActive: true);

    [Fact]
    public void Valid_Command_ShouldPass()
    {
        var result = _sut.TestValidate(ValidCommand());
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Empty_TenantId_ShouldFail()
    {
        var command = ValidCommand() with { TenantId = Guid.Empty };
        var result = _sut.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.TenantId);
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
    public void Invalid_ContactEmail_ShouldFail()
    {
        var command = ValidCommand() with { ContactEmail = "not-valid" };
        var result = _sut.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.ContactEmail);
    }

    [Fact]
    public void Empty_ContactEmail_ShouldFail()
    {
        var command = ValidCommand() with { ContactEmail = "" };
        var result = _sut.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.ContactEmail);
    }
}
