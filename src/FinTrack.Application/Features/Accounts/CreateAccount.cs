using FinTrack.Application.Common.Interfaces;
using FinTrack.Domain.Entities;
using FluentValidation;
using MediatR;

namespace FinTrack.Application.Features.Accounts;

public record CreateAccountCommand(
    string Name,
    string Type,
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

        RuleFor(x => x.Type)
            .NotEmpty().WithMessage("O tipo da conta é obrigatório.")
            .MaximumLength(50).WithMessage("O tipo da conta pode ter no máximo 50 caracteres.");

        RuleFor(x => x.InitialBalance)
            .NotNull().WithMessage("O valor saldo inicial é obrigatório")
            .GreaterThanOrEqualTo(0).WithMessage("O saldo inicial não pode ser negativo.");
    }
}

public class CreateAccountHandler(IAccountRepository accountRepository, IUserContext userContext) : IRequestHandler<CreateAccountCommand, CreateAccountResponse>
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

        // Teste rápido dentro do seu CreateAccountHandler:
        var validator = new CreateAccountValidator(); // substitua pelo nome do seu validador
        var validationResult = await validator.ValidateAsync(request);

        if (!validationResult.IsValid)
        {
            // Se entrar aqui, o validador funciona, mas o MediatR não estava chamando ele sozinho!
            throw new Exception(validationResult.Errors.First().ErrorMessage);
        }


        await accountRepository.AddAsync(account);
        return new CreateAccountResponse(account.Id);
    }
}
