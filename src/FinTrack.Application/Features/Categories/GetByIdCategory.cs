using FinTrack.Application.Common.Interfaces;
using FluentValidation;
using MediatR;

namespace FinTrack.Application.Features.Categories;

public record GetByIdCategoryQuery(Guid Id) : IRequest<GetByIdCategoryResponse>;

public record GetByIdCategoryResponse(
    Guid Id,
    string Name,
    string Type,
    string Color,
    DateTime CreatedAt
    );

public class GetByIdCategoryValidator : AbstractValidator<GetByIdCategoryQuery>
{
    public GetByIdCategoryValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("O ID é obrigatório.");
    }
}
public class GetByIdCategoryHandler(ICategoryRepository categoryRepository, IUserContext userContext) : IRequestHandler<GetByIdCategoryQuery, GetByIdCategoryResponse>
{
    public async Task<GetByIdCategoryResponse> Handle(GetByIdCategoryQuery request, CancellationToken cancellationToken)
    {
        var category = await categoryRepository.GetByIdAsync(request.Id, userContext.UserId, cancellationToken)
            ?? throw new KeyNotFoundException($"Categoria com ID {request.Id} não encontrada");

        return new GetByIdCategoryResponse(
        category.Id,
        category.Name,
        category.Type,
        category.Color,
        category.CreatedAt
        );
    }
}
