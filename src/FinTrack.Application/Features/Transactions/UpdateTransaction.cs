using FinTrack.Application.Common.Interfaces;
using FinTrack.Application.Features.Accounts.Services;
using FinTrack.Domain.Enums;
using FluentValidation;
using MediatR;

namespace FinTrack.Application.Features.Transactions;

public record UpdateTransactionCommand(
    Guid Id,
    Guid AccountId,
    string? Description = null,
    decimal? Amount = null,
    TransactionType? Type = null,
    DateTime? Date = null,
    TransactionStatus? Status = null
) : IRequest<UpdateTransactionResponse>;

public record UpdateTransactionResponse(
    Guid Id,
    string Description,
    decimal Amount,
    TransactionType Type,
    DateTime Date,
    TransactionStatus Status
);

public class UpdateTransactionValidator : AbstractValidator<UpdateTransactionCommand>
{
    public UpdateTransactionValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("O ID da transação é obrigatório.");
        RuleFor(x => x.AccountId)
            .NotEmpty().WithMessage("O ID da conta é obrigatório.");
        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("A descrição da transação é obrigatória.")
            .MaximumLength(100).WithMessage("A descrição da transação pode ter no máximo 100 caracteres.")
            .When(x => x.Description != null);
        RuleFor(x => x.Amount)
            .NotNull().WithMessage("O valor da transação é obrigatório.")
            .GreaterThan(0).WithMessage("O valor da transção deve ser maior que 0.")
            .When(x => x.Amount.HasValue);
        RuleFor(x => x.Date)
            .NotEmpty().WithMessage("A data da transação é inválida.")
            .When(x => x.Date.HasValue);
        RuleFor(x => x.Type)
            .IsInEnum().WithMessage("O tipo de transação é inválido.")
            .When(x => x.Type.HasValue);
        RuleFor(x => x.Status)
            .IsInEnum().WithMessage("O status da transação é inválido")
            .When(x => x.Status.HasValue);
    }
}
public class UpdateTransactionHandler(ITransactionRepository transactionRepository, IAccountRepository accountRepository, ICategoryRepository categoryRepository, IUserContext userContext) : IRequestHandler<UpdateTransactionCommand, UpdateTransactionResponse>
{
    public async Task<UpdateTransactionResponse> Handle(UpdateTransactionCommand request, CancellationToken cancellationToken)
    {
        var transaction = await transactionRepository.GetByIdAsync(request.Id, userContext.UserId, request.AccountId, cancellationToken)
            ?? throw new KeyNotFoundException($"Transação não encontrada.");

        var category = await categoryRepository.GetByIdAsync(transaction.CategoryId, userContext.UserId, cancellationToken)
            ?? throw new KeyNotFoundException($"Categoria não encontrada.");

        var oldAmount = transaction.Amount;
        var oldType = transaction.Type;
        var oldStatus = transaction.Status;

        var account = await accountRepository.GetByIdAsync(
            transaction.AccountId,
            userContext.UserId,
            cancellationToken
        ) ?? throw new KeyNotFoundException("Conta não encontrada.");

        var newAmount = request.Amount ?? oldAmount;
        var newType = request.Type ?? oldType;
        var newStatus = request.Status ?? oldStatus;

        if (category.Type != newType) throw new ArgumentException("O tipo da transação não é igual ao da categoria.");

        if (oldStatus == TransactionStatus.Paid) account.CurrentBalance -= AccountBalanceCalculator.CalculateDelta(oldAmount, oldType);

        if (newStatus == TransactionStatus.Paid) account.CurrentBalance += AccountBalanceCalculator.CalculateDelta(newAmount, newType);

        transaction.Description = request.Description ?? transaction.Description;
        transaction.Amount = newAmount;
        transaction.Type = newType;
        transaction.Date = request.Date ?? transaction.Date;
        transaction.Status = request.Status ?? transaction.Status;

        await transactionRepository.SaveChangesAsync(cancellationToken);

        return new UpdateTransactionResponse(
            transaction.Id,
            transaction.Description,
            transaction.Amount,
            transaction.Type,
            transaction.Date,
            transaction.Status
        );
    }
}
