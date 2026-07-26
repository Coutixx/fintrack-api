using MediatR;

namespace FinTrack.Application.Features.Accounts.CreateAccount;

public record CreateAccountCommand(
    string Name,
    string Type,
    decimal InitialBalance
) : IRequest<CreateAccountResponse>;
