using FinTrack.Application.Features.Categories;
using FinTrack.Domain.Entities;
using FinTrack.Domain.Enums;

namespace FinTrack.Application.Common.Interfaces;

public interface ICategoryRepository
{
    Task AddAsync(Category category);

    Task<Category?> GetByIdAsync(Guid id, Guid userId, CancellationToken cancellationToken);

    Task<List<CategoryItem>> GetAllAsync(Guid userId, TransactionType? type, CancellationToken cancellationToken);

    Task SaveChangesAsync(CancellationToken cancellationToken);

    Task<bool> ExistingByNameAsync(Guid userId, string name, CancellationToken cancellationToken);
}
