using FinTrack.Application.Common.Interfaces;
using FinTrack.Domain.Entities;
using FinTrack.Domain.Enums;
using FluentValidation;
using MediatR;

namespace FinTrack.Application.Features.Categories;

public record CreateCategoryCommand(
    string Name,
    TransactionType Type,
    string Color
) : IRequest<CreateCategoryResponse>;

public record CreateCategoryResponse(Guid Id);

public class CreateCategoryValidator : AbstractValidator<CreateCategoryCommand>
{
    public CreateCategoryValidator(ICategoryRepository categoryRepository, IUserContext userContext)
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("O nome da categoria é obrigatório.")
            .MaximumLength(100).WithMessage("O nome da categoria pode ter no máximo 100 caracteres.")
            .MustAsync(async (name, cancellation) =>
            {
                var userId = userContext.UserId;
                return !await categoryRepository.ExistingByNameAsync(userId, name, cancellation);
            })
            .WithMessage("Já existe uma categoria com esse nome.");
        RuleFor(x => x.Color)
            .NotEmpty().WithMessage("A cor da categoria é obrigatória.")
            .MaximumLength(50).WithMessage("A cor da categoria pode ter no máximo 50 caracteres.");
        RuleFor(x => x.Type)
            .IsInEnum().WithMessage("O tipo da categoria é inválido.");
    }
}

public class CreateCategoryHandler(ICategoryRepository categoryRepository, IUnitOfWork unitOfWork, IUserContext userContext) : IRequestHandler<CreateCategoryCommand, CreateCategoryResponse>
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

        await categoryRepository.AddAsync(category, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return new CreateCategoryResponse(category.Id);
    }
}
