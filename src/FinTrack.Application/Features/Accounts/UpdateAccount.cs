using FinTrack.Application.Common.Interfaces;
using FluentValidation;
using MediatR;

namespace FinTrack.Application.Features.Accounts;

public record UpdateAccountCommand(
    Guid Id,
    string Name,
    string Type
) : IRequest<UpdateAccountResponse>;

public record UpdateAccountResponse(
    Guid Id,
    string Name,
    string Type,
    decimal CurrentBalance,
    DateTime CreatedAt,
    DateTime? UpdatedAt
);

public class UpdateAccountValidator : AbstractValidator<UpdateAccountCommand>
{
    public UpdateAccountValidator()
    {
        RuleFor(x => x.Name)
            .MaximumLength(100).WithMessage("O nome da conta pode ter no máximo 100 caracteres.");

        RuleFor(x => x.Type)
                .MaximumLength(50).WithMessage("O tipo da conta pode ter no máximo 50 caracteres.");
    }
}

public class UpdateAccountHandler(IAccountRepository accountRepository, IUserContext userContext) : IRequestHandler<UpdateAccountCommand, UpdateAccountResponse>
{
    public async Task<UpdateAccountResponse> Handle(UpdateAccountCommand request, CancellationToken cancellationToken)
    {
        var account = await accountRepository.GetByIdAsync(request.Id, userContext.UserId, cancellationToken);
        if (account is null) throw new KeyNotFoundException($"Conta com ID {request.Id} não encontrada");

        account.Name = request.Name;
        account.Type = request.Type;
        account.UpdatedAt = DateTime.UtcNow;

        await accountRepository.SaveChangesAsync(cancellationToken);

        return new UpdateAccountResponse(
        account.Id,
        account.Name,
        account.Type,
        account.CurrentBalance,
        account.CreatedAt,
        account.UpdatedAt
        );
    }
}
