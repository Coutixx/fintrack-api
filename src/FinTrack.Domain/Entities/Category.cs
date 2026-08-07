using FinTrack.Domain.Common;

namespace FinTrack.Domain.Entities;

public class Category : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

}
