using FinTrack.Application.Common.Interfaces;
using FinTrack.Application.Features.Accounts;
using FinTrack.Domain.Entities;
using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace FinTrack.UnitTests.Features.Accounts;

public class GetByIdAccountHandlerTests
{
    private readonly IAccountRepository _accountRepository = Substitute.For<IAccountRepository>();
    private readonly IUserContext _userContext = Substitute.For<IUserContext>();
    private readonly GetByIdAccountHandler _handler;

    public GetByIdAccountHandlerTests() =>
        _handler = new GetByIdAccountHandler(_accountRepository, _userContext);

    [Fact]
    public async Task GetById_ValidId_ReturnsAccount()
    {
        // Arrange
        var id = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var account = new Account { Id = id, UserId = userId };
        _userContext.UserId.Returns(userId);
        _accountRepository.GetByIdAsync(id, userId, Arg.Any<CancellationToken>()).Returns(account);

        // Act
        var response = await _handler.Handle(new GetByIdAccountQuery(id), CancellationToken.None);

        // Assert
        Assert.Equal(id, response.Id);
        Assert.Equal("Nome", response.Name);
    }

    [Fact]
    public async Task GetById_InvalidId_ThrowsNotFound()
    {
        // Arrange
        var id = Guid.NewGuid();
        var userId = Guid.NewGuid();

        _userContext.UserId.Returns(userId);

        _accountRepository.
            GetByIdAsync(id, userId, Arg.Any<CancellationToken>()).
            ReturnsNull();

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            _handler.Handle(new GetByIdAccountQuery(id), CancellationToken.None));
    }
}
