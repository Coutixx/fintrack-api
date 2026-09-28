using FinTrack.Application.Common.Interfaces;
using FinTrack.Application.Features.Categories;
using FinTrack.Domain.Enums;
using NSubstitute;

namespace FinTrack.UnitTests.Features.Categories;

public class UpdateCategoryValidatorTests
{
    private readonly ICategoryRepository _categoryRepository = Substitute.For<ICategoryRepository>();
    private readonly IUserContext _userContext = Substitute.For<IUserContext>();

    private readonly UpdateCategoryValidator _validator;

    public UpdateCategoryValidatorTests() =>
        _validator = new UpdateCategoryValidator(_categoryRepository, _userContext);

    [Fact]
    public async Task Validate_WhenNameIsUnchanged_DoesNotReturnDuplicateError()
    {
        // Arrange
        var id = Guid.NewGuid();
        var userId = Guid.NewGuid();
        _userContext.UserId.Returns(userId);
        _categoryRepository
            .ExistingByNameAsync(userId, "Alimentação", CancellationToken.None, id)
            .Returns(false);
        var request = new UpdateCategoryCommand(
            id,
            "Alimentação",
            TransactionType.Expense,
            "Azul");

        // Act
        var result = await _validator.ValidateAsync(request);

        // Assert
        Assert.True(result.IsValid);
        await _categoryRepository.Received(1)
            .ExistingByNameAsync(userId, "Alimentação", CancellationToken.None, id);
    }

    [Fact]
    public async Task Validate_WhenNameBelongsToAnotherCategory_ReturnsValidationError()
    {
        // Arrange
        var id = Guid.NewGuid();
        var userId = Guid.NewGuid();
        _userContext.UserId.Returns(userId);
        _categoryRepository
            .ExistingByNameAsync(userId, "Alimentação", CancellationToken.None, id)
            .Returns(true);
        var request = new UpdateCategoryCommand(
            id,
            "Alimentação",
            TransactionType.Expense,
            "Azul");

        // Act
        var result = await _validator.ValidateAsync(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(request.Name));
    }
}
