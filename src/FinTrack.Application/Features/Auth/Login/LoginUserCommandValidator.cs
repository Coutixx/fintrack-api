using FluentValidation;

namespace FinTrack.Application.Features.Auth.Login;

public class LoginUserCommandValidator : AbstractValidator<LoginUserCommand>
{
    public LoginUserCommandValidator()
    {
        RuleFor(x => x.Email)
        .NotEmpty().WithMessage("O e-mail é obrigatório.")
        .EmailAddress().WithMessage("Formato de e-mail inválido.")
        .MaximumLength(150).WithMessage("O e-mail deve ter no máximo 150 caracteres.");

        RuleFor(x => x.Password)
        .NotEmpty().WithMessage("A senha é obrigatória.")
        .MinimumLength(8).WithMessage("A senha deve ter pelo menos 8 caracteres.");
    }
}
