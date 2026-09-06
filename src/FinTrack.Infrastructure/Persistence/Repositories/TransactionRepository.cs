using FinTrack.Application.Common.Interfaces;
using FinTrack.Application.Features.Transactions;
using FinTrack.Domain.Entities;
using FinTrack.Domain.Enums;
using FinTrack.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FinTrack.Infrastructure.Persistence.Repositories;

public class TransactionRepository(AppDbContext context) : ITransactionRepository
{
    public async Task AddAsync(Transaction transaction, CancellationToken cancellationToken)
    {
        context.Transactions.Add(transaction);
        await context.SaveChangesAsync(cancellationToken);
    }

    public Task<Transaction?> GetByIdAsync(Guid id, Guid userId, Guid accountId, CancellationToken cancellationToken) =>
        context.Transactions.FirstOrDefaultAsync(t => t.Id == id && t.Account.UserId == userId && t.AccountId == accountId, cancellationToken);

    public async Task<List<TransactionItem>> GetAllAsync(Guid userId, Guid? accountId, TransactionType? type, CancellationToken cancellationToken)
    {
        var query = context.Transactions.AsNoTracking().Where(t => t.Account.UserId == userId);

        if (accountId.HasValue) query = query.Where(t => t.AccountId == accountId);

        if (type.HasValue) query = query.Where(t => t.Type == type);

        return await query.Select(a => new TransactionItem(
            a.Id,
            a.Description,
            a.Amount,
            a.Type,
            a.Date,
            a.Status
        ))
        .ToListAsync(cancellationToken);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken) =>
        await context.SaveChangesAsync(cancellationToken);
}
