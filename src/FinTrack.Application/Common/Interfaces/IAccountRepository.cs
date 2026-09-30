using FinTrack.Application.Common.Models;
using FinTrack.Domain.Entities;

namespace FinTrack.Application.Common.Interfaces;

public interface IAccountRepository
{
    Task AddAsync(Account account, CancellationToken cancellationToken);

    Task<Account?> GetByIdAsync(Guid id, Guid userId, CancellationToken token);

    Task<List<AccountReadModel>> GetAllAsync(Guid userId, CancellationToken cancellationToken);

}
