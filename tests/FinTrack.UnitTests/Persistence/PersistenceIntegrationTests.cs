using FinTrack.Application.Common.Interfaces;
using FinTrack.Application.Features.Categories;
using FinTrack.Application.Features.Transactions;
using FinTrack.Domain.Entities;
using FinTrack.Domain.Enums;
using FinTrack.Infrastructure.Data;
using FinTrack.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using NSubstitute;

namespace FinTrack.UnitTests.Persistence;

public class PersistenceIntegrationTests
{
    [Fact]
    public async Task TransactionListing_AppliesSoftDeleteFiltersAcrossRelatedEntities()
    {
        var options = CreateOptions();
        await using var context = new AppDbContext(options);
        var user = CreateUser();
        var activeAccount = CreateAccount(user);
        var deletedAccount = CreateAccount(user, deleted: true);
        var activeCategory = CreateCategory(user);
        var deletedCategory = CreateCategory(user, deleted: true);
        var visibleHistory = CreateTransaction(activeAccount, deletedCategory);
        var hiddenWithAccount = CreateTransaction(deletedAccount, activeCategory);
        var deletedTransaction = CreateTransaction(activeAccount, activeCategory, deleted: true);

        await context.AddRangeAsync(
            user,
            activeAccount,
            deletedAccount,
            activeCategory,
            deletedCategory,
            visibleHistory,
            hiddenWithAccount,
            deletedTransaction);
        await context.SaveChangesAsync();

        var transactions = await new TransactionRepository(context).GetAllAsync(
            user.Id,
            null,
            null,
            CancellationToken.None);
        var categories = await context.Categories.ToListAsync();
        var accounts = await context.Accounts.ToListAsync();

        Assert.Equal(new[] { visibleHistory.Id }, transactions.Select(transaction => transaction.Id));
        Assert.Single(categories);
        Assert.Equal(activeCategory.Id, categories[0].Id);
        Assert.Single(accounts);
        Assert.Equal(activeAccount.Id, accounts[0].Id);
    }

    [Fact]
    public async Task CreatePaidTransaction_PersistsAccountBalanceAndTransactionTogether()
    {
        var options = CreateOptions();
        await using var context = new AppDbContext(options);
        var user = CreateUser();
        var account = CreateAccount(user);
        var category = CreateCategory(user, type: TransactionType.Income);
        await context.AddRangeAsync(user, account, category);
        await context.SaveChangesAsync();

        var userContext = Substitute.For<IUserContext>();
        userContext.UserId.Returns(user.Id);
        var handler = new CreateTransactionHandler(
            new TransactionRepository(context),
            new AccountRepository(context),
            new CategoryRepository(context),
            context,
            userContext);

        var response = await handler.Handle(
            new CreateTransactionCommand(
                account.Id,
                category.Id,
                "Salário",
                75,
                TransactionType.Income,
                new DateOnly(2026, 09, 30),
                TransactionStatus.Paid),
            CancellationToken.None);

        context.ChangeTracker.Clear();
        var persistedAccount = await context.Accounts.SingleAsync(item => item.Id == account.Id);
        var persistedTransaction = await context.Transactions.SingleAsync(item => item.Id == response.Id);

        Assert.Equal(75, persistedAccount.CurrentBalance);
        Assert.Equal(account.Id, persistedTransaction.AccountId);
        Assert.Equal(category.Id, persistedTransaction.CategoryId);
    }

    [Fact]
    public async Task UpdateTransaction_CanUseSoftDeletedCategoryForExistingHistory()
    {
        var options = CreateOptions();
        await using var context = new AppDbContext(options);
        var user = CreateUser();
        var account = CreateAccount(user, currentBalance: 50);
        var category = CreateCategory(user, deleted: true);
        var transaction = CreateTransaction(account, category, amount: 50, status: TransactionStatus.Pending);
        await context.AddRangeAsync(user, account, category, transaction);
        await context.SaveChangesAsync();

        var userContext = Substitute.For<IUserContext>();
        userContext.UserId.Returns(user.Id);
        var handler = new UpdateTransactionHandler(
            new TransactionRepository(context),
            new AccountRepository(context),
            new CategoryRepository(context),
            context,
            userContext);

        await handler.Handle(
            new UpdateTransactionCommand(
                transaction.Id,
                account.Id,
                "Despesa paga",
                50,
                TransactionType.Expense,
                new DateOnly(2026, 09, 30),
                TransactionStatus.Paid),
            CancellationToken.None);

        context.ChangeTracker.Clear();
        var persistedAccount = await context.Accounts.SingleAsync(item => item.Id == account.Id);
        var persistedTransaction = await context.Transactions.SingleAsync(item => item.Id == transaction.Id);

        Assert.Equal(0, persistedAccount.CurrentBalance);
        Assert.Equal(TransactionStatus.Paid, persistedTransaction.Status);
    }

    [Fact]
    public async Task UpdateCategory_RejectsTypeChangeWhenOnlySoftDeletedTransactionIsLinked()
    {
        var options = CreateOptions();
        await using var context = new AppDbContext(options);
        var user = CreateUser();
        var account = CreateAccount(user);
        var category = CreateCategory(user);
        var originalCategoryName = category.Name;
        var deletedTransaction = CreateTransaction(
            account,
            category,
            deleted: true);
        await context.AddRangeAsync(user, account, category, deletedTransaction);
        await context.SaveChangesAsync();

        var userContext = Substitute.For<IUserContext>();
        userContext.UserId.Returns(user.Id);
        var handler = new UpdateCategoryHandler(
            new CategoryRepository(context),
            context,
            userContext);

        var exception = await Assert.ThrowsAsync<ArgumentException>(() =>
            handler.Handle(
                new UpdateCategoryCommand(
                    category.Id,
                    "New category name",
                    TransactionType.Income,
                    "#ffffff"),
                CancellationToken.None));

        Assert.Equal(
            "Não é possível alterar o tipo da categoria porque existem transações vinculadas.",
            exception.Message);
        Assert.Equal(TransactionType.Expense, category.Type);
        Assert.Equal(originalCategoryName, category.Name);
    }

    private static DbContextOptions<AppDbContext> CreateOptions() =>
        new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

    private static User CreateUser() => new()
    {
        Name = "Test User",
        Email = $"{Guid.NewGuid()}@example.com",
        PasswordHash = "hashed"
    };

    private static Account CreateAccount(User user, bool deleted = false, decimal currentBalance = 0) => new()
    {
        UserId = user.Id,
        User = user,
        Name = "Checking",
        Type = AccountType.Checking,
        CurrentBalance = currentBalance,
        DeletedAt = deleted ? DateTime.UtcNow : null
    };

    private static Category CreateCategory(
        User user,
        bool deleted = false,
        TransactionType type = TransactionType.Expense) => new()
    {
        UserId = user.Id,
        User = user,
        Name = $"Category-{Guid.NewGuid()}",
        Type = type,
        Color = "#000000",
        DeletedAt = deleted ? DateTime.UtcNow : null
    };

    private static Transaction CreateTransaction(
        Account account,
        Category category,
        decimal amount = 100,
        TransactionStatus status = TransactionStatus.Paid,
        bool deleted = false) => new()
    {
        AccountId = account.Id,
        Account = account,
        CategoryId = category.Id,
        Category = category,
        Description = "Transaction",
        Amount = amount,
        Type = TransactionType.Expense,
        Date = new DateOnly(2026, 09, 30),
        Status = status,
        DeletedAt = deleted ? DateTime.UtcNow : null
    };
}
