using FinTrack.Application.Common.Interfaces;
using FinTrack.Domain.Enums;
using MediatR;

namespace FinTrack.Application.Features.Transactions;

public record GetAllTransactionsQuery(Guid? AccountId = null, TransactionType? Type = null) : IRequest<GetAllTransactionsResponse>;

public record TransactionItem(
    Guid Id,
    string Description,
    decimal Amount,
    TransactionType Type,
    DateTime Date,
    TransactionStatus Status
);

public record GetAllTransactionsResponse(List<TransactionItem> Transactions);
public class GetAllTransactionsHandler(ITransactionRepository transactionRepository, IUserContext userContext) : IRequestHandler<GetAllTransactionsQuery, GetAllTransactionsResponse>
{
    public async Task<GetAllTransactionsResponse> Handle(GetAllTransactionsQuery request, CancellationToken cancellationToken)
    {
        var transactions = await transactionRepository.GetAllAsync(userContext.UserId, request.AccountId, request.Type, cancellationToken);

        return new GetAllTransactionsResponse(transactions);
    }
}
