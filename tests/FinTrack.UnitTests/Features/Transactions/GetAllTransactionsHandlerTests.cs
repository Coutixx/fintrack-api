using FinTrack.Application.Common.Interfaces;
using FinTrack.Application.Features.Transactions;
using FinTrack.Domain.Enums;
using NSubstitute;

namespace FinTrack.UnitTests.Features.Transactions;

public class GetAllTransactionsHandlerTests
{
    private readonly ITransactionRepository _transactionRepository = Substitute.For<ITransactionRepository>();
    private readonly IUserContext _userContext = Substitute.For<IUserContext>();

    private readonly GetAllTransactionsHandler _handler;

    public GetAllTransactionsHandlerTests() =>
        _handler = new GetAllTransactionsHandler(_transactionRepository, _userContext);

    [Fact]
    public async Task GetAll_UserHasTransactions_ReturnsList()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var transaction = new TransactionItem(
            Guid.NewGuid(),
            "Salário",
            100,
            TransactionType.Income,
            new DateOnly(2026, 09, 10),
            TransactionStatus.Paid);
        _userContext.UserId.Returns(userId);
        _transactionRepository.GetAllAsync(userId, null, null, CancellationToken.None)
            .Returns(new List<TransactionItem> { transaction });

        // Act
        var response = await _handler.Handle(new GetAllTransactionsQuery(), CancellationToken.None);

        // Assert
        Assert.NotNull(response);
        Assert.Single(response.Transactions);
        Assert.Equal("Salário", response.Transactions.First().Description);
    }

    [Fact]
    public async Task GetAll_UserHasNoTransactions_ReturnsEmptyList()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _userContext.UserId.Returns(userId);
        _transactionRepository.GetAllAsync(userId, null, null, CancellationToken.None)
            .Returns(new List<TransactionItem>());

        // Act
        var response = await _handler.Handle(new GetAllTransactionsQuery(), CancellationToken.None);

        // Assert
        Assert.NotNull(response);
        Assert.Empty(response.Transactions);
    }

    [Fact]
    public async Task GetAll_ValidRequest_FiltersByUserId()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _userContext.UserId.Returns(userId);

        // Act
        await _handler.Handle(new GetAllTransactionsQuery(), CancellationToken.None);

        // Assert
        await _transactionRepository.Received(1).GetAllAsync(userId, null, null, CancellationToken.None);
    }

    [Fact]
    public async Task GetAll_WhenAccountIdIsInformed_PassesAccountIdToRepository()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var accountId = Guid.NewGuid();
        _userContext.UserId.Returns(userId);

        // Act
        await _handler.Handle(new GetAllTransactionsQuery(accountId), CancellationToken.None);

        // Assert
        await _transactionRepository.Received(1).GetAllAsync(userId, accountId, null, CancellationToken.None);
    }

    [Fact]
    public async Task GetAll_WhenTypeIsInformed_PassesTypeToRepository()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var type = TransactionType.Income;
        _userContext.UserId.Returns(userId);

        // Act
        await _handler.Handle(new GetAllTransactionsQuery(null, type), CancellationToken.None);

        // Assert
        await _transactionRepository.Received(1).GetAllAsync(userId, null, type, CancellationToken.None);
    }
}
