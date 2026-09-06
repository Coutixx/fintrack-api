using FinTrack.Application.Common.Interfaces;
using FinTrack.Domain.Enums;
using MediatR;

namespace FinTrack.Application.Features.Categories;

public record GetAllCategoriesQuery(TransactionType? Type = null) : IRequest<GetAllCategoriesResponse>;

public record CategoryItem(
    Guid Id,
    string Name,
    TransactionType Type,
    string Color
);
public record GetAllCategoriesResponse(List<CategoryItem> Categories);

public class GetAllCategoriesHandler(ICategoryRepository categoryRepository, IUserContext userContext) : IRequestHandler<GetAllCategoriesQuery, GetAllCategoriesResponse>
{
    public async Task<GetAllCategoriesResponse> Handle(GetAllCategoriesQuery request, CancellationToken cancellationToken)
    {
        var categories = await categoryRepository.GetAllAsync(userContext.UserId, request.Type, cancellationToken);

        return new GetAllCategoriesResponse(categories);
    }
}
