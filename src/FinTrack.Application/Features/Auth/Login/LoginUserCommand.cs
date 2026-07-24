using MediatR;

namespace FinTrack.Application.Features.Auth.Login;

public record LoginUserCommand(
    string Password,
    string Email
) : IRequest<LoginUserResponse>;
