using FinTrack.Application.Common.Interfaces;
using FinTrack.Domain.Entities;

using Mapster;

using MediatR;

namespace FinTrack.Application.Features.Auth.Register;

public class RegisterUserHandler : IRequestHandler<RegisterUserCommand, RegisterUserResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly ITokenService _tokenService;

    public RegisterUserHandler(IUserRepository userRepository, ITokenService tokenService)
    {
        _userRepository = userRepository;
        _tokenService = tokenService;
    }

    public async Task<RegisterUserResponse> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        if (await _userRepository.ExistingByEmailAsync(request.Email))
            throw new Exception("E-mail já cadastrado.");

        // Mapeamento da request para User
        var user = request.Adapt<User>();

        user.Id = Guid.NewGuid();
        user.PasswordHash = await Task.Run(() => BCrypt.Net.BCrypt.HashPassword(request.Password));
        user.CreatedAt = DateTime.UtcNow;

        var token = _tokenService.GenerateToken(user);

        await _userRepository.AddAsync(user);

        return new RegisterUserResponse(user.Id, token);
    }
}
