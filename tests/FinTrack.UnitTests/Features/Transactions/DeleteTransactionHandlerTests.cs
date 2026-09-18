using FinTrack.Application.Common.Interfaces;
using FinTrack.Application.Features.Transactions;
using FinTrack.Domain.Entities;
using FinTrack.Domain.Enums;
using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace FinTrack.UnitTests.Features.Transactions;

public class DeleteTransactionHandlerTests
{
    private readonly ITransactionRepository _transactionRepository = Substitute.For<ITransactionRepository>();
    private readonly IAccountRepository _accountRepository = Substitute.For<IAccountRepository>();
    private readonly IUserContext _userContext = Substitute.For<IUserContext>();

    private readonly DeleteTransactionHandler _handler;

    public DeleteTransactionHandlerTests() =>
        _handler = new DeleteTransactionHandler(_transactionRepository, _accountRepository, _userContext);

    [Fact]
    public async Task Handle_ValidRequest_DeletesTransaction()
    {
        // Arrange
        var transaction = CreateTransaction(TransactionStatus.Pending, TransactionType.Income, 100);
        var account = CreateAccount(transaction.AccountId, 100);
        ConfigureRepositories(transaction, account);

        // Act
        await _handler.Handle(new DeleteTransactionCommand(transaction.Id, account.Id), CancellationToken.None);

        // Assert
        Assert.NotNull(transaction.DeletedAt);
        await _transactionRepository.Received(1).SaveChangesAsync(CancellationToken.None);
    }

    [Fact]
    public async Task Handle_ValidRequest_ReversesPaidTransactionBalance()
    {
        // Arrange
        var transaction = CreateTransaction(TransactionStatus.Paid, TransactionType.Income, 100);
        var account = CreateAccount(transaction.AccountId, 200);
        ConfigureRepositories(transaction, account);

        // Act
        await _handler.Handle(new DeleteTransactionCommand(transaction.Id, account.Id), CancellationToken.None);

        // Assert
        Assert.Equal(100, account.CurrentBalance);
    }

    [Fact]
    public async Task Handle_ValidRequest_DoesNotChangeBalanceForPendingTransaction()
    {
        // Arrange
        var transaction = CreateTransaction(TransactionStatus.Pending, TransactionType.Expense, 100);
        var account = CreateAccount(transaction.AccountId, 200);
        ConfigureRepositories(transaction, account);

        // Act
        await _handler.Handle(new DeleteTransactionCommand(transaction.Id, account.Id), CancellationToken.None);

        // Assert
        Assert.Equal(200, account.CurrentBalance);
    }

    [Fact]
    public async Task Handle_NonExistingTransaction_ThrowsNotFound()
    {
        // Arrange
        var id = Guid.NewGuid();
        var accountId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        _userContext.UserId.Returns(userId);
        _transactionRepository.GetByIdAsync(id, userId, accountId, CancellationToken.None).ReturnsNull();

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            _handler.Handle(new DeleteTransactionCommand(id, accountId), CancellationToken.None));
        await _transactionRepository.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    private void ConfigureRepositories(Transaction transaction, Account account)
    {
        var userId = Guid.NewGuid();
        _userContext.UserId.Returns(userId);
        _transactionRepository
            .GetByIdAsync(transaction.Id, userId, account.Id, CancellationToken.None)
            .Returns(transaction);
        _accountRepository.GetByIdAsync(account.Id, userId, CancellationToken.None).Returns(account);
    }

    private static Transaction CreateTransaction(TransactionStatus status, TransactionType type, decimal amount) =>
        new()
        {
            Id = Guid.NewGuid(),
            AccountId = Guid.NewGuid(),
            Amount = amount,
            Type = type,
            Status = status
        };

    private static Account CreateAccount(Guid id, decimal balance) =>
        new()
        {
            Id = id,
            CurrentBalance = balance
        };
}
