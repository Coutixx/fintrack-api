using FinTrack.Application.Common.Interfaces;
using FinTrack.Application.Features.Categories;
using FinTrack.Domain.Enums;
using NSubstitute;

namespace FinTrack.UnitTests.Features.Categories;

public class CreateCategoryValidatorTests
{
    private readonly ICategoryRepository _categoryRepository = Substitute.For<ICategoryRepository>();
    private readonly IUserContext _userContext = Substitute.For<IUserContext>();

    private readonly CreateCategoryValidator _validator;

    public CreateCategoryValidatorTests() =>
        _validator = new CreateCategoryValidator(_categoryRepository, _userContext);

    [Fact]
    public async Task Validate_WhenColorIsEmpty_ReturnsValidationError()
    {
        // Arrange
        var request = new CreateCategoryCommand("Alimentação", TransactionType.Expense, string.Empty);

        // Act
        var result = await _validator.ValidateAsync(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(request.Color));
    }

    [Fact]
    public async Task Validate_WhenNameAlreadyExists_ReturnsValidationError()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _userContext.UserId.Returns(userId);
        _categoryRepository
            .ExistingByNameAsync(userId, "Alimentação", CancellationToken.None)
            .Returns(true);
        var request = new CreateCategoryCommand("Alimentação", TransactionType.Expense, "Azul");

        // Act
        var result = await _validator.ValidateAsync(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(request.Name));
    }
}
