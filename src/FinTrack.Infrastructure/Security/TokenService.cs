using System.Text;
using FinTrack.Application.Common.Interfaces;
using FinTrack.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.IdentityModel.JsonWebTokens;
using System.Security.Claims;

namespace FinTrack.Infrastructure.Security;

public class TokenService(IConfiguration configuration) : ITokenService
{
    private readonly JsonWebTokenHandler _tokenHandler = new();
    private readonly SigningCredentials _creds = BuildSigningCredentials(configuration);

    private static SigningCredentials BuildSigningCredentials(IConfiguration config)
    {
        var secret = config["JwtSettings:SECRET"]
            ?? throw new InvalidOperationException("Secret JWT não configurada.");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        return new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
    }

    public string GenerateToken(User user)
    {
        if (user is null) throw new ArgumentNullException(nameof(user));

        var expirationHours = configuration.GetValue<int>("JwtSettings:ExpirationHours");
        if (expirationHours <= 0) expirationHours = 1;

        var descriptor = new SecurityTokenDescriptor
        {
            Claims = new Dictionary<string, object>
            {
                { ClaimTypes.NameIdentifier, user.Id.ToString() },
                { ClaimTypes.Email, user.Email },
                { ClaimTypes.Role, "User" }
            },
            Expires = DateTime.UtcNow.AddHours(expirationHours),
            SigningCredentials = _creds
        };

        return _tokenHandler.CreateToken(descriptor);
    }
}
