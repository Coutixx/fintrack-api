using FluentValidation;

namespace FinTrack.Application.Features.Accounts.CreateAccount;

public class CreateAccountCommandValidator : AbstractValidator<CreateAccountCommand>
{
    public CreateAccountCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("O nome da conta é obrigatório.")
            .MaximumLength(100).WithMessage("O nome da conta pode ter no máximo 100 caracteres.");

        RuleFor(x => x.Type)
            .NotEmpty().WithMessage("O tipo da conta é obrigatório.")
            .MaximumLength(50).WithMessage("O tipo da conta pode ter no máximo 50 caracteres.");

        RuleFor(x => x.InitialBalance)
            .NotEmpty().WithMessage("O valor saldo inicial é obrigatório")
            .GreaterThanOrEqualTo(0).WithMessage("O saldo inicial não pode ser negativo.");
    }
}
