using FinTrack.Application.Features.Categories;

namespace FinTrack.UnitTests.Features.Categories;

public class DeleteCategoryValidatorTests
{
    private readonly DeleteCategoryValidator _validator = new();

    [Fact]
    public void Validate_WhenIdIsEmpty_ReturnsValidationError()
    {
        // Arrange
        var request = new DeleteCategoryCommand(Guid.Empty);

        // Act
        var result = _validator.Validate(request);

        // Assert
        Assert.False(result.IsValid);
    }

    [Fact]
    public void Validate_WhenIdIsValid_ReturnsNoValidationError()
    {
        // Arrange
        var request = new DeleteCategoryCommand(Guid.NewGuid());

        // Act
        var result = _validator.Validate(request);

        // Assert
        Assert.True(result.IsValid);
    }
}
