using FinTrack.Application.Common.Interfaces;
using FinTrack.Application.Features.Transactions;
using FinTrack.Domain.Entities;
using FinTrack.Domain.Enums;
using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace FinTrack.UnitTests.Features.Transactions;

public class GetByIdTransactionHandlerTests
{
    private readonly ITransactionRepository _transactionRepository = Substitute.For<ITransactionRepository>();
    private readonly IUserContext _userContext = Substitute.For<IUserContext>();

    private readonly GetByIdTransactionHandler _handler;

    public GetByIdTransactionHandlerTests() =>
        _handler = new GetByIdTransactionHandler(_transactionRepository, _userContext);

    [Fact]
    public async Task GetById_ValidId_ReturnsTransaction()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var transaction = new Transaction
        {
            Id = Guid.NewGuid(),
            AccountId = Guid.NewGuid(),
            Description = "Salário",
            Amount = 100,
            Type = TransactionType.Income,
            Date = new DateOnly(2026, 09, 10),
            Status = TransactionStatus.Paid
        };
        _userContext.UserId.Returns(userId);
        _transactionRepository
            .GetByIdAsync(transaction.Id, userId, transaction.AccountId, CancellationToken.None)
            .Returns(transaction);

        // Act
        var response = await _handler.Handle(
            new GetByIdTransactionQuery(transaction.Id, transaction.AccountId),
            CancellationToken.None);

        // Assert
        Assert.NotNull(response);
        Assert.Equal(transaction.Id, response.Id);
        Assert.Equal(transaction.Description, response.Description);
        Assert.Equal(transaction.Amount, response.Amount);
        Assert.Equal(transaction.Type, response.Type);
        Assert.Equal(transaction.Date, response.Date);
        Assert.Equal(transaction.Status, response.Status);
    }

    [Fact]
    public async Task GetById_InvalidId_ThrowsNotFound()
    {
        // Arrange
        var id = Guid.NewGuid();
        var accountId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        _userContext.UserId.Returns(userId);
        _transactionRepository.GetByIdAsync(id, userId, accountId, CancellationToken.None).ReturnsNull();

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            _handler.Handle(new GetByIdTransactionQuery(id, accountId), CancellationToken.None));
    }
}
