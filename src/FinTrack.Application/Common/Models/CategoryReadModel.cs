using FinTrack.Domain.Enums;

namespace FinTrack.Application.Common.Models;

public record CategoryReadModel(
    Guid Id,
    string Name,
    TransactionType Type,
    string Color);

public record CategoryPageReadModel(
    List<CategoryReadModel> Categories,
    int TotalCount);
