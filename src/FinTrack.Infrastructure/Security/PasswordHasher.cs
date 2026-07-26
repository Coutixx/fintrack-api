using FinTrack.Application.Common.Interfaces;

namespace FinTrack.Infrastructure.Security;

public class PasswordHasher : IPasswordHasher
{
    public bool Verify(string password, string passwordHash) =>
        BCrypt.Net.BCrypt.Verify(password, passwordHash);

    public string Hash(string password) =>
        BCrypt.Net.BCrypt.HashPassword(password);
}
