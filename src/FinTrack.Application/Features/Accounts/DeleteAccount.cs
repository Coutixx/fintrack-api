using FinTrack.Application.Common.Interfaces;
using MediatR;

namespace FinTrack.Application.Features.Accounts;

public record DeleteAccountCommand(Guid Id) : IRequest<Unit>;

public class DeleteAccountHandler(IAccountRepository accountRepository, IUserContext userContext) : IRequestHandler<DeleteAccountCommand, Unit>
{
    public async Task<Unit> Handle(DeleteAccountCommand request, CancellationToken cancellationToken)
    {
        var account = await accountRepository.GetByIdAsync(request.Id, userContext.UserId, cancellationToken)
            ?? throw new KeyNotFoundException($"Conta com ID {request.Id} não encontrada");

        account.DeletedAt = DateTime.UtcNow;

        await accountRepository.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}
