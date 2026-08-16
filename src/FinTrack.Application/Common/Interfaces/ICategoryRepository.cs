using FinTrack.Domain.Entities;

namespace FinTrack.Application.Common.Interfaces;

public interface ICategoryRepository
{
    Task AddAsync(Category category);

    Task<Category?> GetByIdAsync(Guid id, Guid userId, CancellationToken cancellationToken);

    Task<List<Category>> GetAllAsync(Guid userId, string? type, CancellationToken cancellationToken);

    Task SaveChangesAsync(CancellationToken cancellationToken);

    Task<bool> ExistingByNameAsync(Guid userId, string name, CancellationToken cancellationToken);
}
