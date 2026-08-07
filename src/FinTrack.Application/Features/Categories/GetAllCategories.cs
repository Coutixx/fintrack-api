using FinTrack.Application.Common.Interfaces;
using FinTrack.Domain.Entities;
using MediatR;

namespace FinTrack.Application.Features.Categories;

public record GetAllCategoriesQuery() : IRequest<GetAllCategoriesResponse>;

public record GetAllCategoriesResponse(List<Category> categories);

public class GetAllCategoriesHandler(ICategoryRepository categoryRepository, IUserContext userContext) : IRequestHandler<GetAllCategoriesQuery, GetAllCategoriesResponse>
{
    public async Task<GetAllCategoriesResponse> Handle(GetAllCategoriesQuery request, CancellationToken cancellationToken)
    {
        var categories = await categoryRepository.GetAllAsync(userContext.UserId, cancellationToken);

        return new GetAllCategoriesResponse(categories);
    }
}
