using FinTrack.Application.Features.Transactions;
using FinTrack.Domain.Entities;
using FinTrack.Domain.Enums;

namespace FinTrack.Application.Common.Interfaces;

public interface ITransactionRepository
{
    Task AddAsync(Transaction transaction);

    Task<Transaction?> GetByIdAsync(Guid id, Guid userId, Guid accountId, CancellationToken cancellationToken);

    Task<List<TransactionItem>> GetAllAsync(Guid userId, Guid? accountId, TransactionType? type, CancellationToken cancellationToken);

    Task SaveChangesAsync(CancellationToken cancellationToken);
}
