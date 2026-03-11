using FluentValidation.TestHelper;
using PhysioBook.Application.Tenants.Commands;
using PhysioBook.Application.Tenants.Validators;

namespace PhysioBook.UnitTests.Tenants;

public class CreateTenantValidatorTests
{
    private readonly CreateTenantValidator _sut = new();

    private static CreateTenantCommand ValidCommand() => new(
        Name: "Test Clinic",
        Slug: "test-clinic",
        ContactEmail: "clinic@test.com",
        Phone: null,
        Address: null,
        City: null,
        Subdomain: null,
        AdminEmail: "admin@test.com",
        AdminPassword: "Password1",
        AdminFirstName: "John",
        AdminLastName: "Doe");

    [Fact]
    public void Valid_Command_ShouldPass()
    {
        var result = _sut.TestValidate(ValidCommand());
        result.ShouldNotHaveAnyValidationErrors();
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
    public void Empty_Slug_ShouldFail()
    {
        var command = ValidCommand() with { Slug = "" };
        var result = _sut.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Slug);
    }

    [Theory]
    [InlineData("Test-Clinic")]
    [InlineData("test clinic")]
    [InlineData("test_clinic")]
    [InlineData("test--clinic")]
    [InlineData("-test-clinic")]
    [InlineData("test-clinic-")]
    public void Invalid_Slug_Format_ShouldFail(string slug)
    {
        var command = ValidCommand() with { Slug = slug };
        var result = _sut.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Slug);
    }

    [Theory]
    [InlineData("test-clinic")]
    [InlineData("clinic123")]
    [InlineData("my-test-clinic")]
    public void Valid_Slug_Format_ShouldPass(string slug)
    {
        var command = ValidCommand() with { Slug = slug };
        var result = _sut.TestValidate(command);
        result.ShouldNotHaveValidationErrorFor(x => x.Slug);
    }

    [Fact]
    public void Invalid_ContactEmail_ShouldFail()
    {
        var command = ValidCommand() with { ContactEmail = "not-an-email" };
        var result = _sut.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.ContactEmail);
    }

    [Fact]
    public void Invalid_AdminEmail_ShouldFail()
    {
        var command = ValidCommand() with { AdminEmail = "not-an-email" };
        var result = _sut.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.AdminEmail);
    }

    [Fact]
    public void Short_AdminPassword_ShouldFail()
    {
        var command = ValidCommand() with { AdminPassword = "Pass1" };
        var result = _sut.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.AdminPassword);
    }

    [Fact]
    public void AdminPassword_Without_Uppercase_ShouldFail()
    {
        var command = ValidCommand() with { AdminPassword = "password1" };
        var result = _sut.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.AdminPassword);
    }

    [Fact]
    public void AdminPassword_Without_Number_ShouldFail()
    {
        var command = ValidCommand() with { AdminPassword = "Password" };
        var result = _sut.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.AdminPassword);
    }

    [Fact]
    public void Empty_AdminFirstName_ShouldFail()
    {
        var command = ValidCommand() with { AdminFirstName = "" };
        var result = _sut.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.AdminFirstName);
    }

    [Fact]
    public void Empty_AdminLastName_ShouldFail()
    {
        var command = ValidCommand() with { AdminLastName = "" };
        var result = _sut.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.AdminLastName);
    }
}
