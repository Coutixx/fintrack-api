using FinTrack.Domain.Entities;

namespace FinTrack.Application.Common.Interfaces;

public interface IUserRepository
{
    Task<bool> ExistingByEmailAsync(string email, CancellationToken cancellationToken);
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken);
    Task AddAsync(User user);
}
