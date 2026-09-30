using FinTrack.Application.Common.Interfaces;
using FinTrack.Application.Common.Models;
using FinTrack.Domain.Enums;
using FluentValidation;
using MediatR;

namespace FinTrack.Application.Features.Categories;

public record GetAllCategoriesQuery(
    TransactionType? Type = null,
    int Page = 1,
    int PageSize = 10
) : IRequest<GetAllCategoriesResponse>;

public class GetAllCategoriesValidator : AbstractValidator<GetAllCategoriesQuery>
{
    public GetAllCategoriesValidator()
    {
        RuleFor(x => x.Type)
            .IsInEnum()
            .When(x => x.Type.HasValue)
            .WithMessage("O tipo de categoria é inválido.");
        RuleFor(x => x.Page)
            .GreaterThan(0)
            .WithMessage("A página deve ser maior que zero.");
        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100)
            .WithMessage("O tamanho da página deve estar entre 1 e 100.");
    }
}

public record CategoryItem(
    Guid Id,
    string Name,
    TransactionType Type,
    string Color
);
public record CategoryPage(List<CategoryItem> Categories, int TotalCount);
public record GetAllCategoriesResponse(
    List<CategoryItem> Categories,
    int Page,
    int PageSize,
    int TotalCount
);

public class GetAllCategoriesHandler(ICategoryRepository categoryRepository, IUserContext userContext) : IRequestHandler<GetAllCategoriesQuery, GetAllCategoriesResponse>
{
    public async Task<GetAllCategoriesResponse> Handle(GetAllCategoriesQuery request, CancellationToken cancellationToken)
    {
        var categories = await categoryRepository.GetAllAsync(
            userContext.UserId,
            request.Type,
            request.Page,
            request.PageSize,
            cancellationToken);

        return new GetAllCategoriesResponse(
            categories.Categories.Select(category => new CategoryItem(
                category.Id,
                category.Name,
                category.Type,
                category.Color)).ToList(),
            request.Page,
            request.PageSize,
            categories.TotalCount);
    }
}
