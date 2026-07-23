using MediatR;

namespace FinTrack.Application.Features.Auth.Register;

public record RegisterUserCommand(
    string Name,
    string Email,
    string Password
) : IRequest<RegisterUserResponse>;
