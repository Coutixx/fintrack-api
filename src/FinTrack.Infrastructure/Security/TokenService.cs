using FinTrack.Application.Common.Interfaces;

using Microsoft.Extensions.Configuration;

namespace FinTrack.Infrastructure.Security;

public class TokenService : ITokenService
{
    private readonly IConfiguration _configuration;
    public TokenService(IConfiguration configuration)
    {
        _configuration = configuration;
    }
}
