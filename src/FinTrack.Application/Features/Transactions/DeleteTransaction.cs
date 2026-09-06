using FinTrack.Application.Common.Interfaces;
using FluentValidation;
using MediatR;

namespace FinTrack.Application.Features.Transactions;

public record DeleteTransactionCommand(Guid Id, Guid AccountId) : IRequest<Unit>;

public class DeleteTransactionValidator : AbstractValidator<DeleteTransactionCommand>
{
    public DeleteTransactionValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("O ID da categoria é obrigatório.");
        RuleFor(x => x.AccountId)
            .NotEmpty().WithMessage("O ID da conta é obrigatório.");
    }
}

public class DeleteTransactionHandler(ITransactionRepository transactionRepository, IUserContext userContext) : IRequestHandler<DeleteTransactionCommand, Unit>
{
    public async Task<Unit> Handle(DeleteTransactionCommand request, CancellationToken cancellationToken)
    {
        var transaction = await transactionRepository.GetByIdAsync(request.Id, userContext.UserId, request.AccountId, cancellationToken)
            ?? throw new KeyNotFoundException("Transação não encontrada.");

        transaction.DeletedAt = DateTime.UtcNow;

        await transactionRepository.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}
