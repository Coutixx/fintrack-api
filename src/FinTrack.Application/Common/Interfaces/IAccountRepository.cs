using FinTrack.Application.Features.Accounts;
using FinTrack.Domain.Entities;

namespace FinTrack.Application.Common.Interfaces;

public interface IAccountRepository
{
    Task AddAsync(Account account, CancellationToken cancellationToken);

    Task<Account?> GetByIdAsync(Guid id, Guid userId, CancellationToken token);

    Task<List<AccountItem>> GetAllAsync(Guid userId, CancellationToken cancellationToken);

    Task SaveChangesAsync(CancellationToken cancellationToken);
}
