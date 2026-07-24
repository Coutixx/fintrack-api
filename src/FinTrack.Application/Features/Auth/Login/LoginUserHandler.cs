using FinTrack.Application.Common.Interfaces;

using MediatR;

namespace FinTrack.Application.Features.Auth.Login;

public class LoginUserHandler : IRequestHandler<LoginUserCommand, LoginUserResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly ITokenService _tokenService;

    public LoginUserHandler(IUserRepository userRepository, ITokenService tokenService)
    {
        _userRepository = userRepository;
        _tokenService = tokenService;
    }

    public async Task<LoginUserResponse> Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email);

        if (user is null) throw new Exception("E-mail ou senha inválidos");


        bool isPasswordValid = await Task.Run(() => BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash));

        if (!isPasswordValid) throw new Exception("E-mail ou senha inválidos.");

        var token = _tokenService.GenerateToken(user);

        return new LoginUserResponse(user.Email, user.Name, token);


    }
}
