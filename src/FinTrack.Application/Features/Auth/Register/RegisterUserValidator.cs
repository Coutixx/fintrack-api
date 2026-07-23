using FluentValidation;

namespace FinTrack.Application.Features.Auth.Register;

public class RegisterUserValidator : AbstractValidator<RegisterUserCommand>
{
    public RegisterUserValidator()
    {
        RuleFor(x => x.Name)
        .NotEmpty().WithMessage("O nome é obrigatório.")
        .MaximumLength(150).WithMessage("O nome deve ter no máximo 150 caracteres.");

        RuleFor(x => x.Email)
        .NotEmpty().WithMessage("O email é obrigatório.")
        .MaximumLength(150).WithMessage("O e-mail deve ter no máximo 150 caracteres.")
        .EmailAddress().WithMessage("Formato de e-mail inválido.");

        RuleFor(x => x.Password)
        .NotEmpty().WithMessage("A senha é obrigatória.")
        .MinimumLength(8).WithMessage("A senha deve ter pelo menos 6 caracteres.");
    }
}
