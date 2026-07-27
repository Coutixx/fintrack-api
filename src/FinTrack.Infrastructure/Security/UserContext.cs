using System.Security.Claims;
using FinTrack.Application.Common.Interfaces;
using Microsoft.AspNetCore.Http;

namespace FinTrack.Infrastructure.Security;

public class UserContext : IUserContext
{
    private readonly IHttpContextAccessor _httpContextAcessor;

    public UserContext(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAcessor = httpContextAccessor;
    }

    public Guid UserId
    {
        get
        {
            var userId = _httpContextAcessor.HttpContext?.User?.FindFirst("sub")?.Value
                ?? _httpContextAcessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            return userId != null ? Guid.Parse(userId) : Guid.Empty;
        }
    }

    public bool IsAuthenticated =>
        _httpContextAcessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;
}
