using FinTrack.Domain.Entities;

namespace FinTrack.Application.Common.Interfaces;

public interface IAccountRepository
{
    Task AddAsync(Account account);
    Task<Account?> GetByIdAsync(Guid id, Guid userId, CancellationToken token);
    Task<List<Account>> GetAllAsync(Guid userId, CancellationToken cancellationToke);
}
