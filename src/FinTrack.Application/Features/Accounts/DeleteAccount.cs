using FinTrack.Application.Common.Interfaces;
using FluentValidation;
using MediatR;

namespace FinTrack.Application.Features.Accounts;

public record DeleteAccountCommand(Guid Id) : IRequest<Unit>;

public class DeleteAccountValidator : AbstractValidator<DeleteAccountCommand>
{
    public DeleteAccountValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("O ID da conta é obrigatório.");
    }
}

public class DeleteAccountHandler(IAccountRepository accountRepository, IUnitOfWork unitOfWork, IUserContext userContext) : IRequestHandler<DeleteAccountCommand, Unit>
{
    public async Task<Unit> Handle(DeleteAccountCommand request, CancellationToken cancellationToken)
    {
        var account = await accountRepository.GetByIdAsync(request.Id, userContext.UserId, cancellationToken)
            ?? throw new KeyNotFoundException("Conta não encontrada.");

        account.DeletedAt = DateTime.UtcNow;

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}
