using FinTrack.Domain.Entities;

namespace FinTrack.Application.Common.Interfaces;

public interface ICategoryRepository
{
    Task AddAsync(Category category);

    Task<Category?> GetByIdAsync(Guid id, Guid userId, CancellationToken cancellationToken);

    Task<List<Category>> GetAllAsync(Guid userId, CancellationToken cancellationToken);

    Task SaveChangesAsync(CancellationToken cancellationToken);

}
