using FinTrack.Application.Common.Interfaces;
using FinTrack.Application.Features.Transactions;
using FinTrack.Domain.Entities;
using FinTrack.Domain.Enums;
using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace FinTrack.UnitTests.Features.Transactions;

public class UpdateTransactionHandlerTests
{
    private readonly ITransactionRepository _transactionRepository = Substitute.For<ITransactionRepository>();
    private readonly IAccountRepository _accountRepository = Substitute.For<IAccountRepository>();
    private readonly ICategoryRepository _categoryRepository = Substitute.For<ICategoryRepository>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly IUserContext _userContext = Substitute.For<IUserContext>();

    private readonly UpdateTransactionHandler _handler;

    public UpdateTransactionHandlerTests() =>
        _handler = new UpdateTransactionHandler(_transactionRepository, _accountRepository, _categoryRepository, _unitOfWork, _userContext);

    [Fact]
    public async Task Handle_ValidRequest_UpdatesTransaction()
    {
        // Arrange
        var id = Guid.NewGuid();
        var accountId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var date = new DateOnly(2026, 09, 10);
        var transaction = new Transaction
        {
            Id = id,
            AccountId = accountId,
            CategoryId = categoryId,
            Description = "Descrição antiga",
            Amount = 100,
            Type = TransactionType.Expense,
            Date = date.AddDays(-1),
            Status = TransactionStatus.Pending
        };
        var account = new Account { Id = accountId, UserId = userId, CurrentBalance = 100 };
        var category = new Category { Id = categoryId, UserId = userId, Type = TransactionType.Expense };
        var request = new UpdateTransactionCommand(
            id,
            accountId,
            "Descrição nova",
            50,
            TransactionType.Expense,
            date,
            TransactionStatus.Paid);

        _userContext.UserId.Returns(userId);
        _transactionRepository.GetByIdAsync(id, userId, accountId, CancellationToken.None).Returns(transaction);
        _categoryRepository.GetByIdIncludingDeletedAsync(categoryId, userId, CancellationToken.None).Returns(category);
        _accountRepository.GetByIdAsync(accountId, userId, CancellationToken.None).Returns(account);

        // Act
        var response = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.Equal(id, response.Id);
        Assert.Equal(request.Description, response.Description);
        Assert.Equal(request.Amount, response.Amount);
        Assert.Equal(request.Type, response.Type);
        Assert.Equal(request.Date, response.Date);
        Assert.Equal(request.Status, response.Status);
        await _unitOfWork.Received(1).SaveChangesAsync(CancellationToken.None);
    }

    [Fact]
    public async Task Handle_ValidRequest_RecalculatesAccountBalance()
    {
        // Arrange
        var id = Guid.NewGuid();
        var accountId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var transaction = new Transaction
        {
            Id = id,
            AccountId = accountId,
            CategoryId = categoryId,
            Amount = 100,
            Type = TransactionType.Expense,
            Status = TransactionStatus.Paid
        };
        var account = new Account { Id = accountId, UserId = userId, CurrentBalance = 200 };
        var category = new Category { Id = categoryId, UserId = userId, Type = TransactionType.Expense };
        var request = new UpdateTransactionCommand(
            id,
            accountId,
            "Despesa",
            50,
            TransactionType.Expense,
            new DateOnly(2026, 09, 10),
            TransactionStatus.Paid);

        _userContext.UserId.Returns(userId);
        _transactionRepository.GetByIdAsync(id, userId, accountId, CancellationToken.None).Returns(transaction);
        _categoryRepository.GetByIdIncludingDeletedAsync(categoryId, userId, CancellationToken.None).Returns(category);
        _accountRepository.GetByIdAsync(accountId, userId, CancellationToken.None).Returns(account);

        // Act
        await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.Equal(250, account.CurrentBalance);
    }

    [Theory]
    [InlineData(TransactionStatus.Pending, TransactionStatus.Paid, 150)]
    [InlineData(TransactionStatus.Paid, TransactionStatus.Pending, 300)]
    [InlineData(TransactionStatus.Paid, TransactionStatus.Cancelled, 300)]
    [InlineData(TransactionStatus.Cancelled, TransactionStatus.Paid, 150)]
    [InlineData(TransactionStatus.Pending, TransactionStatus.Pending, 200)]
    [InlineData(TransactionStatus.Cancelled, TransactionStatus.Cancelled, 200)]
    [InlineData(TransactionStatus.Paid, TransactionStatus.Paid, 250)]
    public async Task Handle_ValidStatusTransition_UpdatesBalanceCorrectly(
        TransactionStatus oldStatus,
        TransactionStatus newStatus,
        decimal expectedBalance)
    {
        var id = Guid.NewGuid();
        var accountId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var transaction = new Transaction
        {
            Id = id,
            AccountId = accountId,
            CategoryId = categoryId,
            Amount = 100,
            Type = TransactionType.Expense,
            Status = oldStatus
        };
        var account = new Account { Id = accountId, UserId = userId, CurrentBalance = 200 };
        var category = new Category { Id = categoryId, UserId = userId, Type = TransactionType.Expense };
        var request = new UpdateTransactionCommand(
            id,
            accountId,
            "Despesa atualizada",
            50,
            TransactionType.Expense,
            new DateOnly(2026, 09, 10),
            newStatus);

        _userContext.UserId.Returns(userId);
        _transactionRepository.GetByIdAsync(id, userId, accountId, CancellationToken.None).Returns(transaction);
        _categoryRepository.GetByIdIncludingDeletedAsync(categoryId, userId, CancellationToken.None).Returns(category);
        _accountRepository.GetByIdAsync(accountId, userId, CancellationToken.None).Returns(account);

        await _handler.Handle(request, CancellationToken.None);

        Assert.Equal(expectedBalance, account.CurrentBalance);
        await _unitOfWork.Received(1).SaveChangesAsync(CancellationToken.None);
    }

    [Fact]
    public async Task Handle_NonExistingTransaction_ThrowsNotFound()
    {
        // Arrange
        var id = Guid.NewGuid();
        var accountId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        _userContext.UserId.Returns(userId);
        _transactionRepository
            .GetByIdAsync(id, userId, accountId, CancellationToken.None)
            .ReturnsNull();

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            _handler.Handle(
                new UpdateTransactionCommand(
                    id,
                    accountId,
                    "Descrição",
                    100,
                    TransactionType.Income,
                    new DateOnly(2026, 09, 10),
                    TransactionStatus.Paid),
                CancellationToken.None));
        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
