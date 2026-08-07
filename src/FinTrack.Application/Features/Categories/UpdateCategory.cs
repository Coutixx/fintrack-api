using FinTrack.Application.Common.Interfaces;
using FluentValidation;
using MediatR;

namespace FinTrack.Application.Features.Categories;

public record UpdateCategoryCommand(
    Guid Id,
    string Name,
    string Type,
    string Color
) : IRequest<UpdateCategoryResponse>;

public record UpdateCategoryResponse(
    Guid Id,
    string Name,
    string Type,
    string Color,
    DateTime? UpdatedAt
);

public class UpdateCategoryValidator : AbstractValidator<UpdateCategoryCommand>
{
    public UpdateCategoryValidator()
    {
        RuleFor(x => x.Name)
            .MaximumLength(100).WithMessage("O nome da categoria pode ter no máximo 100 caracteres.");

        RuleFor(x => x.Type)
            .MaximumLength(50).WithMessage("O tipo da categoria pode ter no máximo 50 caracteres.");
    }
}

public class UpdateCategoryHandler(ICategoryRepository categoryRepository, IUserContext userContext) : IRequestHandler<UpdateCategoryCommand, UpdateCategoryResponse>
{
    public async Task<UpdateCategoryResponse> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await categoryRepository.GetByIdAsync(request.Id, userContext.UserId, cancellationToken)
            ?? throw new KeyNotFoundException($"Categoria com ID {request.Id} não encontrada");

        category.Name = request.Name;
        category.Type = request.Type;
        category.UpdatedAt = DateTime.UtcNow;

        await categoryRepository.SaveChangesAsync(cancellationToken);

        return new UpdateCategoryResponse(
        category.Id,
        category.Name,
        category.Type,
        category.Color,
        category.UpdatedAt
        );
    }
}
