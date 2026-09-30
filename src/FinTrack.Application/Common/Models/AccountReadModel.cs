using FinTrack.Domain.Enums;

namespace FinTrack.Application.Common.Models;

public record AccountReadModel(
    Guid Id,
    string Name,
    AccountType Type,
    decimal CurrentBalance);
