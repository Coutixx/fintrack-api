using FinTrack.Domain.Entities;

namespace FinTrack.Application.Common.Interfaces;

public interface ITokenService
{
    string GenerateToken(User user);
}
