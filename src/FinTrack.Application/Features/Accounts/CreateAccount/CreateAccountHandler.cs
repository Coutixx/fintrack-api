using FinTrack.Application.Common.Interfaces;
using FinTrack.Domain.Entities;

using MediatR;

namespace FinTrack.Application.Features.Accounts.CreateAccount;

public class CreateAccountHandler : IRequestHandler<CreateAccountCommand, CreateAccountResponse>
{
    private readonly IAccountRepository _accountRepository;
    private readonly IUserContext _userContext;

    public CreateAccountHandler(IAccountRepository accountRepository, IUserContext userContext)
    {
        _accountRepository = accountRepository;
        _userContext = userContext;
    }

    public async Task<CreateAccountResponse> Handle(CreateAccountCommand request, CancellationToken cancellationToken)
    {
        // Adicione essa linha temporária para inspecionar o valor real no terminal
        Console.WriteLine($"[DEBUG FINTRACK] O ID extraído do contexto é: {_userContext.UserId}");

        var account = new Account {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Type = request.Type,
            InitialBalance = request.InitialBalance,
            CurrentBalance = request.InitialBalance,
            UserId = _userContext.UserId,
            CreatedAt = DateTime.UtcNow
        };

        await _accountRepository.AddAsync(account);
        return new CreateAccountResponse(account.Id);
    }
}
