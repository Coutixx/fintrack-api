using FinTrack.Application.Common.Interfaces;
using FinTrack.Domain.Entities;
using MediatR;

namespace FinTrack.Application.Features.Accounts;

public record GetAllAccountsQuery() : IRequest<GetAllAccountsResponse>;

public record GetAllAccountsResponse(List<Account> accounts);

public class GetAllAccountsHandler(IAccountRepository accountRepository, IUserContext userContext) : IRequestHandler<GetAllAccountsQuery, GetAllAccountsResponse>
{
    public async Task<GetAllAccountsResponse> Handle(GetAllAccountsQuery request, CancellationToken cancellationToken)
    {
        var accounts = await accountRepository.GetAllAsync(userContext.UserId, cancellationToken);

        return new GetAllAccountsResponse(accounts);
    }
}
