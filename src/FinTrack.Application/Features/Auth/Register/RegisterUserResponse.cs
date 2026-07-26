namespace FinTrack.Application.Features.Auth.Register;

public record RegisterUserResponse(
    Guid userId,
    string Token
);

