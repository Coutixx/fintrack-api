using FinTrack.Domain.Enums;

namespace FinTrack.Application.Common.Models;

public record TransactionReadModel(
    Guid Id,
    string Description,
    decimal Amount,
    TransactionType Type,
    DateOnly Date,
    TransactionStatus Status);
