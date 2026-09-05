using FinTrack.Domain.Enums;

namespace FinTrack.Application.Features.Accounts.Services;

public static class AccountBalanceCalculator
{
    public static decimal CalculateNewBalance(decimal currentBalance, decimal amount, TransactionType type)
    {
        return type == TransactionType.Income
            ? currentBalance + amount
            : currentBalance - amount;
    }

    public static decimal CalculateDelta(decimal amount, TransactionType type)
    {
        return type == TransactionType.Income
            ? amount
            : -amount;
    }
}
