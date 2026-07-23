using FinTrack.Domain.Entities;

namespace FinTrack.Application.Common.Interfaces;

public interface IUserRepository
{
    Task<bool> ExistingByEmailAsync(string email);

    Task AddAsync(User user);
}
