using FinTrack.Application.Common.Interfaces;
using FinTrack.Application.Features.Accounts;
using FinTrack.Domain.Entities;
using FinTrack.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FinTrack.Infrastructure.Persistence.Repositories;

public class AccountRepository(AppDbContext context) : IAccountRepository
{
    public async Task AddAsync(Account account)
    {
        context.Accounts.Add(account);
        await context.SaveChangesAsync();
    }

    public Task<Account?> GetByIdAsync(Guid id, Guid userId, CancellationToken cancellationToken) =>
        context.Accounts.FirstOrDefaultAsync(a => a.Id == id && a.UserId == userId, cancellationToken);

    public Task<List<AccountItem>> GetAllAsync(Guid userId, CancellationToken cancellationToken) =>
        context.Accounts.AsNoTracking().Where(a => a.UserId == userId).Select(a => new AccountItem(
            a.Id,
            a.Name,
            a.Type,
            a.CurrentBalance
        ))
        .ToListAsync(cancellationToken);

    public async Task SaveChangesAsync(CancellationToken cancellationToken) =>
        await context.SaveChangesAsync(cancellationToken);
}
