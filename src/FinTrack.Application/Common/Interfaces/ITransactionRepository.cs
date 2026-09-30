using FinTrack.Application.Common.Models;
using FinTrack.Domain.Entities;
using FinTrack.Domain.Enums;

namespace FinTrack.Application.Common.Interfaces;

public interface ITransactionRepository
{
    Task AddAsync(Transaction transaction, CancellationToken cancellationToken);

    Task<Transaction?> GetByIdAsync(Guid id, Guid userId, Guid accountId, CancellationToken cancellationToken);

    Task<List<TransactionReadModel>> GetAllAsync(Guid userId, Guid? accountId, TransactionType? type, CancellationToken cancellationToken);

}
