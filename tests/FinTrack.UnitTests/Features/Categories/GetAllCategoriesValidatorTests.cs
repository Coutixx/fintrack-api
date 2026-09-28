using FinTrack.Application.Features.Categories;
using FinTrack.Domain.Enums;

namespace FinTrack.UnitTests.Features.Categories;

public class GetAllCategoriesValidatorTests
{
    private readonly GetAllCategoriesValidator _validator = new();

    [Fact]
    public void Validate_WhenTypeIsNotInformed_ReturnsNoValidationError()
    {
        // Arrange
        var request = new GetAllCategoriesQuery();

        // Act
        var result = _validator.Validate(request);

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_WhenTypeIsValid_ReturnsNoValidationError()
    {
        // Arrange
        var request = new GetAllCategoriesQuery(TransactionType.Income);

        // Act
        var result = _validator.Validate(request);

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_WhenTypeIsInvalid_ReturnsValidationError()
    {
        // Arrange
        var request = new GetAllCategoriesQuery((TransactionType)999);

        // Act
        var result = _validator.Validate(request);

        // Assert
        Assert.False(result.IsValid);
    }

    [Fact]
    public void Validate_WhenPageIsZero_ReturnsValidationError()
    {
        // Arrange
        var request = new GetAllCategoriesQuery(Page: 0);

        // Act
        var result = _validator.Validate(request);

        // Assert
        Assert.False(result.IsValid);
    }

    [Fact]
    public void Validate_WhenPageSizeExceedsMaximum_ReturnsValidationError()
    {
        // Arrange
        var request = new GetAllCategoriesQuery(PageSize: 101);

        // Act
        var result = _validator.Validate(request);

        // Assert
        Assert.False(result.IsValid);
    }
}
