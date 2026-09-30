using FinTrack.Application.Common.Interfaces;
using FinTrack.Domain.Entities;
using FinTrack.Domain.Enums;
using FluentValidation;
using MediatR;

namespace FinTrack.Application.Features.Accounts;

public record CreateAccountCommand(
    string Name,
    AccountType Type,
    decimal? InitialBalance
) : IRequest<CreateAccountResponse>;

public record CreateAccountResponse(Guid Id);

public class CreateAccountValidator : AbstractValidator<CreateAccountCommand>
{
    public CreateAccountValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("O nome da conta é obrigatório.")
            .MaximumLength(100).WithMessage("O nome da conta pode ter no máximo 100 caracteres.");
        RuleFor(x => x.InitialBalance)
            .GreaterThanOrEqualTo(0).When(x => x.InitialBalance.HasValue)
            .WithMessage("O saldo inicial não pode ser negativo.");
        RuleFor(x => x.Type)
            .IsInEnum().WithMessage("O tipo de conta é inválido.");
    }
}

public class CreateAccountHandler(IAccountRepository accountRepository, IUnitOfWork unitOfWork, IUserContext userContext) : IRequestHandler<CreateAccountCommand, CreateAccountResponse>
{

    public async Task<CreateAccountResponse> Handle(CreateAccountCommand request, CancellationToken cancellationToken)
    {
        var account = new Account
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Type = request.Type,
            InitialBalance = request.InitialBalance ?? 0,
            CurrentBalance = request.InitialBalance ?? 0,
            UserId = userContext.UserId,
            CreatedAt = DateTime.UtcNow
        };

        await accountRepository.AddAsync(account, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return new CreateAccountResponse(account.Id);
    }
}
