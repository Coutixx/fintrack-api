using FinTrack.Application.Common.Interfaces;
using FinTrack.Application.Features.Transactions;
using NSubstitute;
using FinTrack.Domain.Enums;
using FinTrack.Domain.Entities;

namespace FinTrack.UnitTests.Features.Transactions;

public class CreateTransactionHandlerTests
{
    private readonly ITransactionRepository _transactionRepository = Substitute.For<ITransactionRepository>();
    IAccountRepository _accountRepository = Substitute.For<IAccountRepository>();
    ICategoryRepository _categoryRepository = Substitute.For<ICategoryRepository>();
    private readonly IUserContext _userContext = Substitute.For<IUserContext>();

    private readonly CreateTransactionHandler _handler;

    public CreateTransactionHandlerTests() =>
        _handler = new CreateTransactionHandler(_transactionRepository, _accountRepository, _categoryRepository, _userContext);

    [Fact]
    public async Task Handle_WhenCommandIsValid_ReturnsTransactionId()
    {
        // Arrange
        var accountId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var request = new CreateTransactionCommand(
            accountId,
            categoryId,
            "Fatura",
            234.8m,
            TransactionType.Income,
            new DateTime(2026, 09, 09),
            TransactionStatus.Paid
        );
        var account = new Account
        {
            Id = accountId,
            UserId = userId,
            CurrentBalance = 100
        };
        var category = new Category
        {
            Id = categoryId,
            UserId = userId,
            Type = request.Type
        };
        _userContext.UserId.Returns(userId);
        _accountRepository.GetByIdAsync(accountId, userId, CancellationToken.None).Returns(account);
        _categoryRepository.GetByIdAsync(categoryId, userId, CancellationToken.None).Returns(category);

        // Act
        var response = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.NotNull(response);
        Assert.NotEqual(Guid.Empty, response.Id);
        Assert.NotEqual(Guid.Empty, response.AccountId);
        await _transactionRepository.Received(1).AddAsync(Arg.Is<Transaction>(t =>
        t.AccountId == request.AccountId &&
        t.CategoryId == request.CategoryId &&
        t.Description == request.Description &&
        t.Type == request.Type &&
        t.Date == request.Date &&
        t.Status == request.Status
        ), CancellationToken.None);
    }

    [Fact]
    public async Task Handle_WhenCommandIsValid_IncreasesBalanceForPaidIncome()
    {
        // Arrange
        var accountId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var account = new Account { Id = accountId, UserId = userId, CurrentBalance = 100 };
        var category = new Category { Id = categoryId, UserId = userId, Type = TransactionType.Income };
        var request = new CreateTransactionCommand(
            accountId,
            categoryId,
            "Salário",
            200,
            TransactionType.Income,
            DateTime.UtcNow,
            TransactionStatus.Paid);
        _userContext.UserId.Returns(userId);
        _accountRepository.GetByIdAsync(accountId, userId, CancellationToken.None).Returns(account);
        _categoryRepository.GetByIdAsync(categoryId, userId, CancellationToken.None).Returns(category);

        // Act
        await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.Equal(300, account.CurrentBalance);
    }

    [Fact]
    public async Task Handle_WhenCommandIsValid_DecreasesBalanceForPaidExpense()
    {
        // Arrange
        var accountId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var account = new Account { Id = accountId, UserId = userId, CurrentBalance = 300 };
        var category = new Category { Id = categoryId, UserId = userId, Type = TransactionType.Expense };
        var request = new CreateTransactionCommand(
            accountId,
            categoryId,
            "Aluguel",
            200,
            TransactionType.Expense,
            DateTime.UtcNow,
            TransactionStatus.Paid);
        _userContext.UserId.Returns(userId);
        _accountRepository.GetByIdAsync(accountId, userId, CancellationToken.None).Returns(account);
        _categoryRepository.GetByIdAsync(categoryId, userId, CancellationToken.None).Returns(category);

        // Act
        await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.Equal(100, account.CurrentBalance);
    }

    [Fact]
    public async Task Handle_WhenTransactionIsPending_DoesNotChangeBalance()
    {
        // Arrange
        var accountId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var account = new Account { Id = accountId, UserId = userId, CurrentBalance = 300 };
        var category = new Category { Id = categoryId, UserId = userId, Type = TransactionType.Income };
        var request = new CreateTransactionCommand(
            accountId,
            categoryId,
            "Receita futura",
            200,
            TransactionType.Income,
            DateTime.UtcNow,
            TransactionStatus.Pending);
        _userContext.UserId.Returns(userId);
        _accountRepository.GetByIdAsync(accountId, userId, CancellationToken.None).Returns(account);
        _categoryRepository.GetByIdAsync(categoryId, userId, CancellationToken.None).Returns(category);

        // Act
        await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.Equal(300, account.CurrentBalance);
    }

    [Fact]
    public async Task Handle_WhenCategoryTypeDiffersFromTransactionType_ThrowsArgumentException()
    {
        // Arrange
        var accountId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var account = new Account { Id = accountId, UserId = userId };
        var category = new Category { Id = categoryId, UserId = userId, Type = TransactionType.Expense };
        var request = new CreateTransactionCommand(
            accountId,
            categoryId,
            "Incompatível",
            200,
            TransactionType.Income,
            DateTime.UtcNow,
            TransactionStatus.Paid);
        _userContext.UserId.Returns(userId);
        _accountRepository.GetByIdAsync(accountId, userId, CancellationToken.None).Returns(account);
        _categoryRepository.GetByIdAsync(categoryId, userId, CancellationToken.None).Returns(category);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() =>
            _handler.Handle(request, CancellationToken.None));
        await _transactionRepository.DidNotReceive().AddAsync(Arg.Any<Transaction>(), CancellationToken.None);
    }
}
