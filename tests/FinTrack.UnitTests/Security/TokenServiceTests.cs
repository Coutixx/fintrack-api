using System.Text;
using FinTrack.Domain.Entities;
using FinTrack.Infrastructure.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace FinTrack.UnitTests.Security;

public class TokenServiceTests
{
    [Fact]
    public async Task GenerateToken_UsesConfiguredUtf8KeyIssuerAndAudience()
    {
        const string secret = "fintrack-test-secret-with-utf8-ç-value-long-enough";
        const string issuer = "FinTrack.Api.Tests";
        const string audience = "FinTrack.Client.Tests";
        var configuration = CreateConfiguration(secret, issuer, audience, 2);
        var service = new TokenService(configuration);

        var token = service.GenerateToken(new User
        {
            Id = Guid.NewGuid(),
            Email = "user@example.com"
        });

        var handler = new JsonWebTokenHandler();
        var result = await handler.ValidateTokenAsync(token, new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret)),
            ValidateIssuer = true,
            ValidIssuer = issuer,
            ValidateAudience = true,
            ValidAudience = audience,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        });

        Assert.True(result.IsValid, result.Exception?.Message);
    }

    [Theory]
    [InlineData("short", "FinTrack.Api", "FinTrack.Client", 1)]
    [InlineData("a sufficiently long test secret", "", "FinTrack.Client", 1)]
    [InlineData("a sufficiently long test secret", "FinTrack.Api", "", 1)]
    [InlineData("a sufficiently long test secret", "FinTrack.Api", "FinTrack.Client", 0)]
    public void Constructor_InvalidJwtSettings_Throws(
        string secret,
        string issuer,
        string audience,
        int expirationHours)
    {
        var configuration = CreateConfiguration(secret, issuer, audience, expirationHours);

        Assert.Throws<InvalidOperationException>(() => new TokenService(configuration));
    }

    private static IConfiguration CreateConfiguration(
        string secret,
        string issuer,
        string audience,
        int expirationHours) =>
        new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["JwtSettings:SECRET"] = secret,
                ["JwtSettings:Issuer"] = issuer,
                ["JwtSettings:Audience"] = audience,
                ["JwtSettings:ExpirationHours"] = expirationHours.ToString()
            })
            .Build();
}
