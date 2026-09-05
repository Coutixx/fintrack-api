using FinTrack.Application.Common.Interfaces;
using FinTrack.Domain.Entities;
using FinTrack.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FinTrack.Infrastructure.Persistence.Repositories;

public class TransactionRepository(AppDbContext context) : ITransactionRepository
{
    public async Task AddAsync(Transaction transaction)
    {
        context.Transactions.Add(transaction);
        await context.SaveChangesAsync();
    }

    public Task<Transaction?> GetByIdAsync(Guid id, Guid accountId, CancellationToken cancellationToken) =>
        context.Transactions.FirstOrDefaultAsync(a => a.Id == id && a.AccountId == accountId, cancellationToken);

    public Task<List<Transaction>> GetAllAsync(Guid accountId, CancellationToken cancellationToken) =>
        context.Transactions.AsNoTracking().Where(a =>
        a.AccountId == accountId).ToListAsync(cancellationToken);

    public async Task SaveChangesAsync(CancellationToken cancellationToken) =>
        await context.SaveChangesAsync(cancellationToken);
}
