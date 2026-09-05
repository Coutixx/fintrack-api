using FinTrack.Domain.Entities;

namespace FinTrack.Application.Common.Interfaces;

public interface ITransactionRepository
{
    Task AddAsync(Transaction transaction);

    Task<Transaction?> GetByIdAsync(Guid id, Guid accountId, CancellationToken cancellationToken);

    Task<List<Transaction>> GetAllAsync(Guid accountId, CancellationToken cancellationToken);

    Task SaveChangesAsync(CancellationToken cancellationToken);
}
