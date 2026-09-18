using FinTrack.Application.Features.Transactions;
using FinTrack.Domain.Enums;

namespace FinTrack.UnitTests.Features.Transactions;

public class GetAllTransactionsValidatorTests
{
    private readonly GetAllTransactionsValidator _validator = new();

    [Fact]
    public void Validate_WhenFiltersAreNotInformed_ReturnsNoValidationError()
    {
        // Arrange
        var request = new GetAllTransactionsQuery();

        // Act
        var result = _validator.Validate(request);

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_WhenFiltersAreValid_ReturnsNoValidationError()
    {
        // Arrange
        var request = new GetAllTransactionsQuery(Guid.NewGuid(), TransactionType.Expense);

        // Act
        var result = _validator.Validate(request);

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_WhenTypeIsInvalid_ReturnsValidationError()
    {
        // Arrange
        var request = new GetAllTransactionsQuery(null, (TransactionType)999);

        // Act
        var result = _validator.Validate(request);

        // Assert
        Assert.False(result.IsValid);
    }
}
