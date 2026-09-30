using FinTrack.Application.Common.Interfaces;
using FinTrack.Application.Common.Models;
using FinTrack.Application.Features.Accounts;
using FinTrack.Domain.Enums;
using NSubstitute;

namespace FinTrack.UnitTests.Features.Accounts;

public class GetAllAccountsHandlerTests
{
    private readonly IAccountRepository _accountRepository = Substitute.For<IAccountRepository>();
    private readonly IUserContext _userContext = Substitute.For<IUserContext>();
    private readonly GetAllAccountsHandler _handler;

    public GetAllAccountsHandlerTests() =>
        _handler = new GetAllAccountsHandler(_accountRepository, _userContext);

    [Fact]
    public async Task GetAll_UserHasAccounts_ReturnsList()
    {
        // Arrange
        var id = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var account = new AccountReadModel(
            id,
            "Conta",
            AccountType.Checking,
            2332
        );
        var accounts = new List<AccountReadModel> { account };

        _userContext.UserId.Returns(userId);
        _accountRepository.GetAllAsync(userId, Arg.Any<CancellationToken>()).Returns(accounts);

        // Act
        var response = await _handler.Handle(new GetAllAccountsQuery(), CancellationToken.None);

        // Assert
        Assert.NotNull(response);
        Assert.Single(response.Accounts);
        Assert.Equal("Conta", response.Accounts.First().Name);

    }

    [Fact]
    public async Task GetAll_UserHasNoAccounts_ReturnsEmptyList()
    {
        // Arrange
        var id = Guid.NewGuid();
        _userContext.UserId.Returns(id);
        _accountRepository.GetAllAsync(id, Arg.Any<CancellationToken>()).Returns(new List<AccountReadModel>());

        // Act
        var response = await _handler.Handle(new GetAllAccountsQuery(), CancellationToken.None);

        // Assert
        Assert.NotNull(response);
        Assert.Empty(response.Accounts);
    }

    [Fact]
    public async Task GetAll_ValidRequest_FiltersByUserId()
    {
        // Arrange
        var id = Guid.NewGuid();
        _userContext.UserId.Returns(id);
        _accountRepository.GetAllAsync(id, CancellationToken.None).Returns(new List<AccountReadModel>());

        // Act
        await _handler.Handle(new GetAllAccountsQuery(), CancellationToken.None);

        // Assert
        await _accountRepository.Received(1).GetAllAsync(id, CancellationToken.None);
    }
}
