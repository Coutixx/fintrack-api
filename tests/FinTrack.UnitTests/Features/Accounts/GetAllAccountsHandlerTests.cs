using FinTrack.Application.Common.Interfaces;
using FinTrack.Application.Features.Accounts;
using FinTrack.Domain.Entities;
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
        var account = new Account { Name = "Conta", Type = "Tipo", UserId = id };
        var accounts = new List<Account> { account };

        _userContext.UserId.Returns(id);
        _accountRepository.GetAllAsync(id, Arg.Any<CancellationToken>()).Returns(accounts);

        // Act
        var response = await _handler.Handle(new GetAllAccountsQuery(), CancellationToken.None);

        // Assert
        Assert.NotNull(response);
        Assert.Single(response.accounts);
        Assert.Equal("Conta", response.accounts.First().Name);

    }

    [Fact]
    public async Task GetAll_UserHasNoAccounts_ReturnsEmptyList()
    {
        // Arrange
        var id = Guid.NewGuid();
        _userContext.UserId.Returns(id);
        _accountRepository.GetAllAsync(id, Arg.Any<CancellationToken>()).Returns(new List<Account>());

        // Act
        var response = await _handler.Handle(new GetAllAccountsQuery(), CancellationToken.None);

        // Assert
        Assert.NotNull(response);
        Assert.Empty(response.accounts);
    }

    [Fact]
    public async Task GetAll_ValidRequest_FiltersByUserId()
    {
        // Arrange
        var id = Guid.NewGuid();
        _userContext.UserId.Returns(id);

        // Act
        await _handler.Handle(new GetAllAccountsQuery(), CancellationToken.None);

        // Assert
        await _accountRepository.Received(1).GetAllAsync(id, CancellationToken.None);
    }
}
