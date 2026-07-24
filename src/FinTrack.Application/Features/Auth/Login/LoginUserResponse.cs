namespace FinTrack.Application.Features.Auth.Login;

public record LoginUserResponse(

    string Email,
    string Name,
    string Token
);
