using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Options;
using PhysioBook.Domain.Entities;
using PhysioBook.Infrastructure.Auth;
using PhysioBook.Infrastructure.Services;

namespace PhysioBook.UnitTests.Auth;

public class JwtServiceTests
{
    private readonly JwtService _sut;
    private readonly JwtSettings _settings;

    public JwtServiceTests()
    {
        _settings = new JwtSettings
        {
            Secret = "test-secret-key-that-is-at-least-32-characters-long!!",
            Issuer = "test-issuer",
            Audience = "test-audience",
            AccessTokenExpirationMinutes = 15,
        };
        _sut = new JwtService(Options.Create(_settings), new DateTimeProvider());
    }

    [Fact]
    public void GenerateAccessToken_ReturnsValidJwt()
    {
        var user = CreateTestUser();

        var token = _sut.GenerateAccessToken(user);

        Assert.NotEmpty(token);
        var handler = new JwtSecurityTokenHandler();
        Assert.True(handler.CanReadToken(token));
    }

    [Fact]
    public void GenerateAccessToken_ContainsCorrectClaims()
    {
        var user = CreateTestUser();

        var token = _sut.GenerateAccessToken(user);

        var handler = new JwtSecurityTokenHandler();
        var jwt = handler.ReadJwtToken(token);

        Assert.Equal(user.Id.ToString(), jwt.Subject);
        Assert.Equal(user.Email, jwt.Claims.First(c => c.Type == "email").Value);
        Assert.Equal(user.TenantId.ToString(), jwt.Claims.First(c => c.Type == "tenant_id").Value);
        Assert.Equal(user.Role, jwt.Claims.First(c => c.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role").Value);
    }

    [Fact]
    public void GenerateRefreshToken_ReturnsUniqueTokens()
    {
        var token1 = _sut.GenerateRefreshToken();
        var token2 = _sut.GenerateRefreshToken();

        Assert.NotEmpty(token1);
        Assert.NotEmpty(token2);
        Assert.NotEqual(token1, token2);
    }

    [Fact]
    public void ValidateAccessToken_ReturnsTrueForValidToken()
    {
        var user = CreateTestUser();
        var token = _sut.GenerateAccessToken(user);

        Assert.True(_sut.ValidateAccessToken(token));
    }

    [Fact]
    public void ValidateAccessToken_ReturnsFalseForInvalidToken()
    {
        Assert.False(_sut.ValidateAccessToken("invalid-token"));
    }

    private static User CreateTestUser() => new()
    {
        Id = Guid.NewGuid(),
        TenantId = Guid.NewGuid(),
        Email = "test@example.com",
        PasswordHash = "hash",
        FirstName = "Test",
        LastName = "User",
        Role = "therapist",
        IsTherapist = true,
        IsActive = true,
    };
}
