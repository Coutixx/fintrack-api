using FinTrack.Application.Common.Interfaces;
using FinTrack.Domain.Entities;
using FinTrack.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FinTrack.Infrastructure.Persistence.Repositories;

public class CategoryRepository(AppDbContext context) : ICategoryRepository
{
    public async Task AddAsync(Category category)
    {
        context.Categories.Add(category);
        await context.SaveChangesAsync();
    }

    public Task<Category?> GetByIdAsync(Guid id, Guid userId, CancellationToken cancellationToken) =>
        context.Categories.FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId, cancellationToken);

    public Task<List<Category>> GetAllAsync(Guid userId, CancellationToken cancellationToken) =>
        context.Categories.AsNoTracking().Where(a => a.UserId == userId).ToListAsync(cancellationToken);

    public async Task SaveChangesAsync(CancellationToken cancellationToken) =>
        await context.SaveChangesAsync(cancellationToken);

    public async Task<bool> ExistingByNameAsync(Guid userId, string name, CancellationToken cancellationToken) =>
        await context.Categories.
            AnyAsync(c => c.UserId == userId && c.Name == name && c.DeletedAt == null, cancellationToken);

}
