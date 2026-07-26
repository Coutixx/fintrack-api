using System.Security.Authentication;

using FinTrack.Application.Common.Interfaces;
using FinTrack.Domain.Entities;

using MediatR;

namespace FinTrack.Application.Features.Auth.Register;

public class RegisterUserHandler : IRequestHandler<RegisterUserCommand, RegisterUserResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly ITokenService _tokenService;
    private readonly IPasswordHasher _passwordHasher;

    public RegisterUserHandler(IUserRepository userRepository, ITokenService tokenService, IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _tokenService = tokenService;
        _passwordHasher = passwordHasher;
    }

    public async Task<RegisterUserResponse> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        if (await _userRepository.ExistingByEmailAsync(request.Email, cancellationToken))
            throw new InvalidCredentialException("E-mail já cadastrado.");

        var PasswordHash = _passwordHasher.Hash(request.Password);

        var user = new User {
            Email = request.Email,
            Name = request.Name,
            PasswordHash = PasswordHash,
            Id = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow
        };

        await _userRepository.AddAsync(user);

        var token = _tokenService.GenerateToken(user);

        return new RegisterUserResponse(user.Id, token);
    }
}
