using FinTrack.Domain.Common;
using FinTrack.Domain.Enums;

namespace FinTrack.Domain.Entities;

public class Category : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public TransactionType Type { get; set; }
    public string Color { get; set; } = string.Empty;
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

}
