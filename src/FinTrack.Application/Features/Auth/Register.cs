using System.Security.Authentication;
using FinTrack.Application.Common.Interfaces;
using FinTrack.Domain.Entities;
using FluentValidation;
using MediatR;

namespace FinTrack.Application.Features.Auth;

public record RegisterCommand(
    string Name,
    string Email,
    string Password
) : IRequest<RegisterResponse>;

public record RegisterResponse(Guid userId, string Token);

public class RegisterValidator : AbstractValidator<RegisterCommand>
{
    public RegisterValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("O nome é obrigatório.")
            .MaximumLength(150).WithMessage("O nome deve ter no máximo 150 caracteres.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("O e-mail é obrigatório.")
            .MaximumLength(150).WithMessage("O e-mail deve ter no máximo 150 caracteres.")
            .EmailAddress().WithMessage("Formato de e-mail inválido.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("A senha é obrigatória.")
            .MinimumLength(8).WithMessage("A senha deve ter pelo menos 8 caracteres.");
    }
}

public class RegisterHandler(IUserRepository userRepository, ITokenService tokenService, IPasswordHasher passwordHasher) : IRequestHandler<RegisterCommand, RegisterResponse>
{
    public async Task<RegisterResponse> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        if (await userRepository.ExistingByEmailAsync(request.Email, cancellationToken))
            throw new InvalidCredentialException("E-mail já cadastrado.");

        var PasswordHash = passwordHasher.Hash(request.Password);

        var user = new User
        {
            Email = request.Email,
            Name = request.Name,
            PasswordHash = PasswordHash,
            Id = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow
        };

        await userRepository.AddAsync(user);

        var token = tokenService.GenerateToken(user);

        return new RegisterResponse(user.Id, token);
    }
}

