using PhysioBook.Infrastructure.Auth;

namespace PhysioBook.UnitTests.Auth;

public class PasswordServiceTests
{
    private readonly PasswordService _sut = new();

    [Fact]
    public void Hash_ReturnsNonEmptyString()
    {
        var hash = _sut.Hash("TestPassword123");
        Assert.NotEmpty(hash);
    }

    [Fact]
    public void Hash_ReturnsDifferentHashesForSamePassword()
    {
        var hash1 = _sut.Hash("TestPassword123");
        var hash2 = _sut.Hash("TestPassword123");
        Assert.NotEqual(hash1, hash2); // bcrypt uses random salt
    }

    [Fact]
    public void Verify_ReturnsTrueForCorrectPassword()
    {
        var hash = _sut.Hash("TestPassword123");
        Assert.True(_sut.Verify("TestPassword123", hash));
    }

    [Fact]
    public void Verify_ReturnsFalseForWrongPassword()
    {
        var hash = _sut.Hash("TestPassword123");
        Assert.False(_sut.Verify("WrongPassword", hash));
    }

    [Fact]
    public void Hash_UsesBcryptFormat()
    {
        var hash = _sut.Hash("TestPassword123");
        Assert.StartsWith("$2a$12$", hash); // bcrypt cost factor 12
    }
}
