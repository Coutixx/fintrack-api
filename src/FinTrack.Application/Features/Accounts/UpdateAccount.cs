using FinTrack.Application.Common.Interfaces;
using FinTrack.Domain.Enums;
using FluentValidation;
using MediatR;

namespace FinTrack.Application.Features.Accounts;

public record UpdateAccountCommand(
    Guid Id,
    string Name,
    AccountType Type
) : IRequest<UpdateAccountResponse>;

public record UpdateAccountResponse(
    Guid Id,
    string Name,
    AccountType Type,
    decimal CurrentBalance
);

public class UpdateAccountValidator : AbstractValidator<UpdateAccountCommand>
{
    public UpdateAccountValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("O nome da conta é obrigatório.")
            .MaximumLength(100).WithMessage("O nome da conta pode ter no máximo 100 caracteres.");
        RuleFor(x => x.Type)
            .IsInEnum().WithMessage("O tipo de conta é inválido.");
    }
}

public class UpdateAccountHandler(IAccountRepository accountRepository, IUserContext userContext) : IRequestHandler<UpdateAccountCommand, UpdateAccountResponse>
{
    public async Task<UpdateAccountResponse> Handle(UpdateAccountCommand request, CancellationToken cancellationToken)
    {
        var account = await accountRepository.GetByIdAsync(request.Id, userContext.UserId, cancellationToken)
            ?? throw new KeyNotFoundException($"Conta com ID {request.Id} não encontrada");

        account.Name = request.Name;
        account.Type = request.Type;

        account.UpdatedAt = DateTime.UtcNow;

        await accountRepository.SaveChangesAsync(cancellationToken);

        return new UpdateAccountResponse(
        account.Id,
        account.Name,
        account.Type,
        account.CurrentBalance
        );
    }
}
