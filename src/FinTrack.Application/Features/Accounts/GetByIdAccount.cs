using FinTrack.Application.Common.Interfaces;
using FluentValidation;
using MediatR;

namespace FinTrack.Application.Features.Accounts;

public record GetByIdAccountQuery(Guid Id) : IRequest<GetByIdAccountResponse>;

public record GetByIdAccountResponse(
    Guid Id,
    string Name,
    string Type,
    decimal CurrentBalance,
    DateTime CreatedAt
    );

public class GetByIdAccountValidator : AbstractValidator<GetByIdAccountQuery>
{
    public GetByIdAccountValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("O ID é obrigatório.");
    }
}
public class GetByIdAccountHandler(IAccountRepository accountsRepository, IUserContext userContext) : IRequestHandler<GetByIdAccountQuery, GetByIdAccountResponse>
{
    public async Task<GetByIdAccountResponse> Handle(GetByIdAccountQuery request, CancellationToken cancellationToken)
    {
        var account = await accountsRepository.GetByIdAsync(request.Id, userContext.UserId, cancellationToken);
        if (account is null) throw new KeyNotFoundException($"Conta com ID {request.Id} não encontrada");

        return new GetByIdAccountResponse(
        account.Id,
        account.Name,
        account.Type,
        account.CurrentBalance,
        account.CreatedAt
        );
    }
}
