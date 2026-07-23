using FinTrack.Application.Common.Interfaces;
using FinTrack.Domain.Entities;

using Mapster;

using MediatR;

namespace FinTrack.Application.Features.Auth.Register;

public class RegisterUserHandler : IRequestHandler<RegisterUserCommand, RegisterUserResponse>
{
    private readonly IUserRepository _userRepository;

    public RegisterUserHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
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

        string token = "token";

        await _userRepository.AddAsync(user);

        return new RegisterUserResponse(user.Id, token);
    }
}
