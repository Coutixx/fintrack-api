using FinTrack.Application.Common.Interfaces;
using FinTrack.Domain.Enums;
using MediatR;

namespace FinTrack.Application.Features.Accounts;

public record GetAllAccountsQuery() : IRequest<GetAllAccountsResponse>;

public record AccountItem(
    Guid Id,
    string Name,
    AccountType Type,
    decimal CurrentBalance
);
public record GetAllAccountsResponse(List<AccountItem> Accounts);

public class GetAllAccountsHandler(IAccountRepository accountRepository, IUserContext userContext) : IRequestHandler<GetAllAccountsQuery, GetAllAccountsResponse>
{
    public async Task<GetAllAccountsResponse> Handle(GetAllAccountsQuery request, CancellationToken cancellationToken)
    {
        var accounts = await accountRepository.GetAllAsync(userContext.UserId, cancellationToken);

        return new GetAllAccountsResponse(accounts);
    }
}
