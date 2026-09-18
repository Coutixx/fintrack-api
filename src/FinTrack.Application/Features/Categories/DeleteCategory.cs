using FinTrack.Application.Common.Interfaces;
using FluentValidation;
using MediatR;

namespace FinTrack.Application.Features.Categories;

public record DeleteCategoryCommand(Guid Id) : IRequest<Unit>;

public class DeleteCategoryValidator : AbstractValidator<DeleteCategoryCommand>
{
    public DeleteCategoryValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("O ID da categoria é obrigatório.");
    }
}

public class DeleteCategoryHandler(ICategoryRepository categoryRepository, IUserContext userContext) : IRequestHandler<DeleteCategoryCommand, Unit>
{
    public async Task<Unit> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await categoryRepository.GetByIdAsync(request.Id, userContext.UserId, cancellationToken)
            ?? throw new KeyNotFoundException($"Categoria não encontrada.");

        category.DeletedAt = DateTime.UtcNow;

        await categoryRepository.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}
