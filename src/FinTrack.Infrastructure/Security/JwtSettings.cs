using System.Text;
using Microsoft.Extensions.Configuration;

namespace FinTrack.Infrastructure.Security;

public sealed record JwtSettings(
    string Secret,
    string Issuer,
    string Audience,
    int ExpirationHours)
{
    public byte[] SigningKey => Encoding.UTF8.GetBytes(Secret);

    public static JwtSettings Load(IConfiguration configuration)
    {
        var secret = configuration["JwtSettings:SECRET"];
        if (string.IsNullOrWhiteSpace(secret) || Encoding.UTF8.GetByteCount(secret) < 32)
            throw new InvalidOperationException("A chave JWT deve conter pelo menos 32 bytes UTF-8.");

        var issuer = configuration["JwtSettings:Issuer"];
        if (string.IsNullOrWhiteSpace(issuer))
            throw new InvalidOperationException("Issuer JWT não configurado.");

        var audience = configuration["JwtSettings:Audience"];
        if (string.IsNullOrWhiteSpace(audience))
            throw new InvalidOperationException("Audience JWT não configurada.");

        var expirationHours = configuration.GetValue<int?>("JwtSettings:ExpirationHours");
        if (!expirationHours.HasValue || expirationHours.Value <= 0)
            throw new InvalidOperationException("A expiração JWT deve ser configurada em horas positivas.");

        return new JwtSettings(secret, issuer, audience, expirationHours.Value);
    }
}
