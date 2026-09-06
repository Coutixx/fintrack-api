using FinTrack.Application.Common.Interfaces;
using FinTrack.Application.Features.Accounts.Services;
using FinTrack.Domain.Enums;
using FluentValidation;
using MediatR;

namespace FinTrack.Application.Features.Transactions;

public record DeleteTransactionCommand(Guid Id, Guid AccountId) : IRequest<Unit>;

public class DeleteTransactionValidator : AbstractValidator<DeleteTransactionCommand>
{
    public DeleteTransactionValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("O ID da transação é obrigatório.");
        RuleFor(x => x.AccountId)
            .NotEmpty().WithMessage("O ID da conta é obrigatório.");
    }
}

public class DeleteTransactionHandler(ITransactionRepository transactionRepository, IAccountRepository accountRepository, IUserContext userContext) : IRequestHandler<DeleteTransactionCommand, Unit>
{
    public async Task<Unit> Handle(DeleteTransactionCommand request, CancellationToken cancellationToken)
    {
        var transaction = await transactionRepository.GetByIdAsync(request.Id, userContext.UserId, request.AccountId, cancellationToken)
            ?? throw new KeyNotFoundException("Transação não encontrada.");

        var account = await accountRepository.GetByIdAsync(request.AccountId, userContext.UserId, cancellationToken)
            ?? throw new KeyNotFoundException("Conta não encontrada.");

        if (transaction.Status == TransactionStatus.Paid) account.CurrentBalance -= AccountBalanceCalculator.CalculateDelta(transaction.Amount, transaction.Type);

        transaction.DeletedAt = DateTime.UtcNow;

        await transactionRepository.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}
