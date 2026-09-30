using FinTrack.Application.Common.Interfaces;
using FinTrack.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.IdentityModel.JsonWebTokens;
using System.Security.Claims;

namespace FinTrack.Infrastructure.Security;

public class TokenService : ITokenService
{
    private readonly JwtSettings _settings;
    private readonly JsonWebTokenHandler _tokenHandler = new();
    private readonly SigningCredentials _creds;

    public TokenService(IConfiguration configuration)
    {
        _settings = JwtSettings.Load(configuration);
        _creds = new SigningCredentials(
            new SymmetricSecurityKey(_settings.SigningKey),
            SecurityAlgorithms.HmacSha256);
    }

    public string GenerateToken(User user)
    {
        if (user is null) throw new ArgumentNullException(nameof(user));

        var descriptor = new SecurityTokenDescriptor
        {
            Issuer = _settings.Issuer,
            Audience = _settings.Audience,
            Claims = new Dictionary<string, object>
            {
                { ClaimTypes.NameIdentifier, user.Id.ToString() },
                { ClaimTypes.Email, user.Email },
                { ClaimTypes.Role, "User" }
            },
            Expires = DateTime.UtcNow.AddHours(_settings.ExpirationHours),
            SigningCredentials = _creds
        };

        return _tokenHandler.CreateToken(descriptor);
    }
}
