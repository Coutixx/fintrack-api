using FinTrack.Application.Features.Accounts;
using FinTrack.Domain.Enums;

namespace FinTrack.UnitTests.Features.Accounts;

public class CreateAccountValidatorTests
{
    private readonly CreateAccountValidator _validator = new();

    [Fact]
    public void Validate_WhenInitialBalanceIsNotInformed_ReturnsNoValidationError()
    {
        // Arrange
        var request = new CreateAccountCommand("Conta", AccountType.Checking, null);

        // Act
        var result = _validator.Validate(request);

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_WhenInitialBalanceIsZero_ReturnsNoValidationError()
    {
        // Arrange
        var request = new CreateAccountCommand("Conta", AccountType.Checking, 0);

        // Act
        var result = _validator.Validate(request);

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_WhenInitialBalanceIsNegative_ReturnsValidationError()
    {
        // Arrange
        var request = new CreateAccountCommand("Conta", AccountType.Checking, -1);

        // Act
        var result = _validator.Validate(request);

        // Assert
        Assert.False(result.IsValid);
    }
}
