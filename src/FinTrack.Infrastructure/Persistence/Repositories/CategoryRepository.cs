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

    public Task<List<Category>> GetAllAsync(Guid userId, string? type, CancellationToken cancellationToken)
    {
        var query = context.Categories.AsNoTracking().Where(a => a.UserId == userId);

        if (!string.IsNullOrWhiteSpace(type)) query = query.Where(c => c.Type == type);

        return query.Take(10).ToListAsync(cancellationToken);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken) =>
        await context.SaveChangesAsync(cancellationToken);

    public async Task<bool> ExistingByNameAsync(Guid userId, string name, CancellationToken cancellationToken) =>
        await context.Categories.
            AsNoTracking().AnyAsync(c => c.UserId == userId && c.Name == name, cancellationToken);
}
