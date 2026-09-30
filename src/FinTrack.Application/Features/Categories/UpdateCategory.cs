using FinTrack.Application.Common.Interfaces;
using FinTrack.Domain.Enums;
using FluentValidation;
using MediatR;

namespace FinTrack.Application.Features.Categories;

public record UpdateCategoryRequest(
    string Name,
    TransactionType Type,
    string Color
);

public record UpdateCategoryCommand(
    Guid Id,
    string Name,
    TransactionType Type,
    string Color
) : IRequest<UpdateCategoryResponse>;

public record UpdateCategoryResponse(
    Guid Id,
    string Name,
    TransactionType Type,
    string Color
);

public class UpdateCategoryValidator : AbstractValidator<UpdateCategoryCommand>
{
    public UpdateCategoryValidator(ICategoryRepository categoryRepository, IUserContext userContext)
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("O nome da categoria é obrigatório.")
            .MaximumLength(100).WithMessage("O nome da categoria pode ter no máximo 100 caracteres.")
            .MustAsync(async (command, name, cancellation) =>
            {
                var userId = userContext.UserId;
                return !await categoryRepository.ExistingByNameAsync(
                    userId,
                    name,
                    cancellation,
                    command.Id);
            })
            .WithMessage("Já existe uma categoria com esse nome.");
        RuleFor(x => x.Type)
            .IsInEnum().WithMessage("O tipo da categoria é inválido.");
        RuleFor(x => x.Color)
            .NotEmpty().WithMessage("A cor da categoria é obrigatória.")
            .MaximumLength(50).WithMessage("A cor da categoria pode ter no máximo 50 caracteres.");
    }
}

public class UpdateCategoryHandler(ICategoryRepository categoryRepository, IUnitOfWork unitOfWork, IUserContext userContext) : IRequestHandler<UpdateCategoryCommand, UpdateCategoryResponse>
{
    public async Task<UpdateCategoryResponse> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await categoryRepository.GetByIdAsync(request.Id, userContext.UserId, cancellationToken)
            ?? throw new KeyNotFoundException($"Categoria não encontrada.");

        if (category.Type != request.Type &&
            await categoryRepository.HasTransactionsAsync(category.Id, cancellationToken))
        {
            throw new ArgumentException("Não é possível alterar o tipo da categoria porque existem transações vinculadas.");
        }

        category.Name = request.Name;
        category.Type = request.Type;
        category.Color = request.Color;

        category.UpdatedAt = DateTime.UtcNow;

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new UpdateCategoryResponse(
        category.Id,
        category.Name,
        category.Type,
        category.Color
        );
    }
}
