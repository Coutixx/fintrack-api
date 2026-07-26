using FinTrack.Domain.Common;

namespace FinTrack.Domain.Entities;

public class Account : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public decimal InitialBalance { get; set; }
    public decimal CurrentBalance { get; set; }
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
}
