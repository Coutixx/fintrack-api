using FinTrack.Application.Common.Interfaces;
using FinTrack.Application.Features.Categories;
using FinTrack.Domain.Entities;
using FinTrack.Domain.Enums;
using FinTrack.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FinTrack.Infrastructure.Persistence.Repositories;

public class CategoryRepository(AppDbContext context) : ICategoryRepository
{
    public async Task AddAsync(Category category, CancellationToken cancellationToken)
    {
        context.Categories.Add(category);
        await context.SaveChangesAsync(cancellationToken);
    }

    public Task<Category?> GetByIdAsync(Guid id, Guid userId, CancellationToken cancellationToken) =>
        context.Categories.FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId, cancellationToken);

    public Task<List<CategoryItem>> GetAllAsync(Guid userId, TransactionType? type, CancellationToken cancellationToken)
    {
        var query = context.Categories.AsNoTracking().Where(a => a.UserId == userId);

        if (type.HasValue) query = query.Where(c => c.Type == type);

        return query.Select(a => new CategoryItem(
            a.Id,
            a.Name,
            a.Type,
            a.Color
        )).Take(10).ToListAsync(cancellationToken);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken) =>
        await context.SaveChangesAsync(cancellationToken);

    public async Task<bool> ExistingByNameAsync(Guid userId, string name, CancellationToken cancellationToken) =>
        await context.Categories.
            AsNoTracking().AnyAsync(c => c.UserId == userId && c.Name == name, cancellationToken);
}
