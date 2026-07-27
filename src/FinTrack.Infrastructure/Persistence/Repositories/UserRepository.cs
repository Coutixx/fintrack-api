using FinTrack.Application.Common.Interfaces;
using FinTrack.Domain.Entities;
using FinTrack.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FinTrack.Infrastructure.Persistence.Repositories;

public class UserRepository(AppDbContext context) : IUserRepository
{
    public async Task AddAsync(User user)
    {
        context.Users.Add(user);
        await context.SaveChangesAsync();
    }

    public Task<bool> ExistingByEmailAsync(string email, CancellationToken cancellationToken) =>
        context.Users.AnyAsync(u => u.Email == email, cancellationToken);

    public Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken) =>
         context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
}
