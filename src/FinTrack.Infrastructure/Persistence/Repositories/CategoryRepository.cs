using FinTrack.Application.Common.Interfaces;
using FinTrack.Application.Common.Models;
using FinTrack.Domain.Entities;
using FinTrack.Domain.Enums;
using FinTrack.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FinTrack.Infrastructure.Persistence.Repositories;

public class CategoryRepository(AppDbContext context) : ICategoryRepository
{
    public Task AddAsync(Category category, CancellationToken cancellationToken)
    {
        context.Categories.Add(category);
        return Task.CompletedTask;
    }

    public Task<Category?> GetByIdAsync(Guid id, Guid userId, CancellationToken cancellationToken) =>
        context.Categories.FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId, cancellationToken);

    public async Task<CategoryPageReadModel> GetAllAsync(
        Guid userId,
        TransactionType? type,
        int page,
        int pageSize,
        CancellationToken cancellationToken)
    {
        var query = context.Categories.AsNoTracking().Where(category => category.UserId == userId);

        if (type.HasValue) query = query.Where(category => category.Type == type);

        var totalCount = await query.CountAsync(cancellationToken);
        var skip = (int)Math.Min((long)(page - 1) * pageSize, int.MaxValue);
        var categories = await query
            .OrderBy(category => category.Name)
            .ThenBy(category => category.Id)
            .Skip(skip)
            .Take(pageSize)
            .Select(category => new CategoryReadModel(
                category.Id,
                category.Name,
                category.Type,
                category.Color))
            .ToListAsync(cancellationToken);

        return new CategoryPageReadModel(categories, totalCount);
    }

    public async Task<bool> ExistingByNameAsync(
        Guid userId,
        string name,
        CancellationToken cancellationToken,
        Guid? excludedCategoryId = null) =>
        await context.Categories
            .AsNoTracking()
            .AnyAsync(
                c => c.UserId == userId &&
                    c.Name == name &&
                    (!excludedCategoryId.HasValue || c.Id != excludedCategoryId.Value),
                cancellationToken);
}
