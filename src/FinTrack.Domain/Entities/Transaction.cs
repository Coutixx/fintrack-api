using FinTrack.Domain.Common;
using FinTrack.Domain.Enums;

namespace FinTrack.Domain.Entities;

public class Transaction : BaseEntity
{
    public string Description { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public TransactionType Type { get; set; }
    public DateOnly Date { get; set; }
    public TransactionStatus Status { get; set; }
    public Guid AccountId { get; set; }
    public Account Account { get; set; } = null!;
    public Guid CategoryId { get; set; }
    public Category Category { get; set; } = null!;
}
