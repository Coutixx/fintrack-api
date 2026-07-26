using System.Security.Authentication;

using FinTrack.Application.Common.Interfaces;

using MediatR;

namespace FinTrack.Application.Features.Auth.Login;

public class LoginUserHandler : IRequestHandler<LoginUserCommand, LoginUserResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly ITokenService _tokenService;
    private readonly IPasswordHasher _passwordHasher;

    public LoginUserHandler(IUserRepository userRepository, ITokenService tokenService, IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _tokenService = tokenService;
        _passwordHasher = passwordHasher;
    }

    public async Task<LoginUserResponse> Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);
        if (user is null) throw new InvalidCredentialException("E-mail ou senha inválidos");

        bool isPasswordValid = _passwordHasher.Verify(request.Password, user.PasswordHash);
        if (!isPasswordValid) throw new InvalidCredentialException("E-mail ou senha inválidos.");

        var token = _tokenService.GenerateToken(user);

        return new LoginUserResponse(user.Email, user.Name, token);
    }
}
