using FinTrack.Application.Common.Interfaces;
using FinTrack.Domain.Enums;
using FluentValidation;
using MediatR;

namespace FinTrack.Application.Features.Transactions;

public record GetByIdTransactionQuery(Guid Id, Guid AccountId) : IRequest<GetByIdTransactionResponse>;

public record GetByIdTransactionResponse(
    Guid Id,
    string Description,
    decimal Amount,
    TransactionType Type,
    DateTime Date,
    TransactionStatus Status
);

public class GetByIdTransactionValidator : AbstractValidator<GetByIdTransactionQuery>
{
    public GetByIdTransactionValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("O ID da transação é obrigatório.");
        RuleFor(x => x.AccountId)
            .NotEmpty().WithMessage("O ID da conta é obrigatório");
    }
}
public class GetByIdTransactionHandler(ITransactionRepository transactionRepository, IUserContext userContext) : IRequestHandler<GetByIdTransactionQuery, GetByIdTransactionResponse>
{
    public async Task<GetByIdTransactionResponse> Handle(GetByIdTransactionQuery request, CancellationToken cancellationToken)
    {
        var transaction = await transactionRepository.GetByIdAsync(request.Id, userContext.UserId, request.AccountId, cancellationToken)
            ?? throw new KeyNotFoundException($"Transação não encontrada.");

        return new GetByIdTransactionResponse(
            transaction.Id,
            transaction.Description,
            transaction.Amount,
            transaction.Type,
            transaction.Date,
            transaction.Status
        );
    }
}
