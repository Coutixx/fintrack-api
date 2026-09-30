using FinTrack.Application.Common.Interfaces;
using FinTrack.Application.Common.Models;
using FinTrack.Domain.Entities;
using FinTrack.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FinTrack.Infrastructure.Persistence.Repositories;

public class AccountRepository(AppDbContext context) : IAccountRepository
{
    public Task AddAsync(Account account, CancellationToken cancellationToken)
    {
        context.Accounts.Add(account);
        return Task.CompletedTask;
    }

    public Task<Account?> GetByIdAsync(Guid id, Guid userId, CancellationToken cancellationToken) =>
        context.Accounts.FirstOrDefaultAsync(a => a.Id == id && a.UserId == userId, cancellationToken);

    public Task<List<AccountReadModel>> GetAllAsync(Guid userId, CancellationToken cancellationToken) =>
        context.Accounts.AsNoTracking().Where(a => a.UserId == userId).Select(a => new AccountReadModel(
            a.Id,
            a.Name,
            a.Type,
            a.CurrentBalance
        ))
        .ToListAsync(cancellationToken);

}
