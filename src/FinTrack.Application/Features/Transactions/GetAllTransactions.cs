using FinTrack.Application.Common.Interfaces;
using FinTrack.Domain.Enums;
using FluentValidation;
using MediatR;

namespace FinTrack.Application.Features.Transactions;

public record GetAllTransactionsQuery(Guid? AccountId = null, TransactionType? Type = null) : IRequest<GetAllTransactionsResponse>;

public class GetAllTransactionsValidator : AbstractValidator<GetAllTransactionsQuery>
{
    public GetAllTransactionsValidator()
    {
        RuleFor(x => x.AccountId)
            .NotEmpty()
            .When(x => x.AccountId.HasValue)
            .WithMessage("O ID da conta é inválido.");
        RuleFor(x => x.Type)
            .IsInEnum()
            .When(x => x.Type.HasValue)
            .WithMessage("O tipo de transação é inválido.");
    }
}

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
