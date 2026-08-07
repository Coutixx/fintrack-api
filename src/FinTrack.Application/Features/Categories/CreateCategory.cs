using FinTrack.Application.Common.Interfaces;
using FinTrack.Domain.Entities;
using FluentValidation;
using MediatR;

namespace FinTrack.Application.Features.Categories;

public record CreateCategoryCommand(
    string Name,
    string Type,
    string Color
) : IRequest<CreateCategoryResponse>;

public record CreateCategoryResponse(Guid Id);

public class CreateCategoryValidator : AbstractValidator<CreateCategoryCommand>
{
    public CreateCategoryValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("O nome da categoria é obrigatório.")
            .MaximumLength(100).WithMessage("O nome da categoria pode ter no máximo 100 caracteres.");

        RuleFor(x => x.Type)
            .NotEmpty().WithMessage("O tipo da categoria é obrigatório.")
            .MaximumLength(50).WithMessage("O tipo da categoria pode ter no máximo 50 caracteres.");

        RuleFor(x => x.Color)
            .NotNull().WithMessage("A cor da categoria é obrigatória")
            .MaximumLength(50).WithMessage("A cor da categoria pode ter no máximo 50 caracteres.");
    }
}

public class CreateCategoryHandler(ICategoryRepository categoryRepository, IUserContext userContext) : IRequestHandler<CreateCategoryCommand, CreateCategoryResponse>
{

    public async Task<CreateCategoryResponse> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = new Category
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Type = request.Type,
            Color = request.Color,
            UserId = userContext.UserId,
            CreatedAt = DateTime.UtcNow
        };

        await categoryRepository.AddAsync(category);
        return new CreateCategoryResponse(category.Id);
    }
}
