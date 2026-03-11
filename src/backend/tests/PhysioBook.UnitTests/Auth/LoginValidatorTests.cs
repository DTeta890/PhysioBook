using FluentValidation.TestHelper;
using PhysioBook.Application.Auth.Commands;
using PhysioBook.Application.Auth.Validators;

namespace PhysioBook.UnitTests.Auth;

public class LoginValidatorTests
{
    private readonly LoginValidator _sut = new();

    [Fact]
    public void Valid_Login_ShouldPass()
    {
        var command = new LoginCommand("user@test.com", "password123", Guid.NewGuid());
        var result = _sut.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Empty_Email_ShouldFail()
    {
        var command = new LoginCommand("", "password123", Guid.NewGuid());
        var result = _sut.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public void Invalid_Email_ShouldFail()
    {
        var command = new LoginCommand("not-an-email", "password123", Guid.NewGuid());
        var result = _sut.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public void Empty_Password_ShouldFail()
    {
        var command = new LoginCommand("user@test.com", "", Guid.NewGuid());
        var result = _sut.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Password);
    }

    [Fact]
    public void Empty_TenantId_ShouldFail()
    {
        var command = new LoginCommand("user@test.com", "password123", Guid.Empty);
        var result = _sut.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.TenantId);
    }
}
