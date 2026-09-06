using FinTrack.Application.Common.Interfaces;
using FinTrack.Application.Features.Accounts.Services;
using FinTrack.Domain.Entities;
using FinTrack.Domain.Enums;
using FluentValidation;
using MediatR;

namespace FinTrack.Application.Features.Transactions;

public record CreateTransactionCommand(
    Guid AccountId,
    Guid CategoryId,
    string Description,
    decimal Amount,
    TransactionType Type,
    DateTime Date,
    TransactionStatus Status
) : IRequest<CreateTransactionResponse>;

public record CreateTransactionResponse(Guid Id);

public class CreateTransactionValidator : AbstractValidator<CreateTransactionCommand>
{
    public CreateTransactionValidator()
    {
        RuleFor(x => x.AccountId)
            .NotEmpty().WithMessage("O ID da conta é obrigatório");
        RuleFor(x => x.CategoryId)
            .NotEmpty().WithMessage("O ID da categoria é obrigatório");
        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("A descrição da transação é obrigatória.")
            .MaximumLength(100).WithMessage("A descrição da transação pode ter no máximo 100 caracteres.");
        RuleFor(x => x.Amount)
            .NotNull().WithMessage("O valor da transação é obrigatório.")
            .GreaterThan(0).WithMessage("O valor da transão deve ser maior que 0.");
        RuleFor(x => x.Date)
            .NotEmpty().WithMessage("A data da transação é obrigatória.");
        RuleFor(x => x.Type)
            .IsInEnum().WithMessage("O tipo de transação é inválido.");
        RuleFor(x => x.Status)
            .IsInEnum().WithMessage("O status da transação é inválido");
    }
}

public class CreateTransactionHandler(ITransactionRepository transactionRepository, IAccountRepository accountRepository, ICategoryRepository categoryRepository, IUserContext userContext) : IRequestHandler<CreateTransactionCommand, CreateTransactionResponse>
{
    public async Task<CreateTransactionResponse> Handle(CreateTransactionCommand request, CancellationToken cancellationToken)
    {
        var account = await accountRepository.GetByIdAsync(request.AccountId, userContext.UserId, cancellationToken)
            ?? throw new KeyNotFoundException($"Conta com ID: {request.AccountId} não encontrada.");

        var category = await categoryRepository.GetByIdAsync(request.CategoryId, userContext.UserId, cancellationToken)
            ?? throw new KeyNotFoundException($"Categoria com ID: {request.CategoryId} não encontrada.");

        if (category.Type != request.Type) throw new ArgumentException("O tipo da transação não é igual o da categoria.");

        if (request.Status == TransactionStatus.Paid) account.CurrentBalance = AccountBalanceCalculator.CalculateNewBalance(
            account.CurrentBalance,
            request.Amount,
            request.Type
        );

        var transaction = new Transaction
        {
            Id = Guid.NewGuid(),
            Description = request.Description,
            Amount = request.Amount,
            Type = request.Type,
            Date = request.Date,
            Status = request.Status,
            CreatedAt = DateTime.UtcNow,
            AccountId = request.AccountId,
            CategoryId = request.CategoryId
        };

        await transactionRepository.AddAsync(transaction);
        return new CreateTransactionResponse(transaction.Id);
    }
}
