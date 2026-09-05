using FinTrack.Application.Common.Interfaces;
using FinTrack.Domain.Enums;
using FluentValidation;
using MediatR;

namespace FinTrack.Application.Features.Categories;

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
            .IsInEnum().WithMessage("O tipo de transação enviado é inválido.");
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
        category.Color = request.Color;
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
