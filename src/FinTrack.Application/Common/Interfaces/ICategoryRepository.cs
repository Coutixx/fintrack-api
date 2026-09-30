using FinTrack.Application.Common.Models;
using FinTrack.Domain.Entities;
using FinTrack.Domain.Enums;

namespace FinTrack.Application.Common.Interfaces;

public interface ICategoryRepository
{
    Task AddAsync(Category category, CancellationToken cancellationToken);

    Task<Category?> GetByIdAsync(Guid id, Guid userId, CancellationToken cancellationToken);

    Task<Category?> GetByIdIncludingDeletedAsync(Guid id, Guid userId, CancellationToken cancellationToken);

    Task<CategoryPageReadModel> GetAllAsync(
        Guid userId,
        TransactionType? type,
        int page,
        int pageSize,
        CancellationToken cancellationToken);

    Task<bool> ExistingByNameAsync(
        Guid userId,
        string name,
        CancellationToken cancellationToken,
        Guid? excludedCategoryId = null);

    Task<bool> HasTransactionsAsync(Guid categoryId, CancellationToken cancellationToken);
}
