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
        var account = new Account
        {
            Id = id,
            UserId = userId,
            Name = "Nome",
            Type = "Tipo",
            CurrentBalance = 123.45m,
            CreatedAt = DateTime.UtcNow.AddDays(-1)
        };
        _userContext.UserId.Returns(userId);
        _accountRepository.GetByIdAsync(id, userId, Arg.Any<CancellationToken>()).Returns(account);

        // Act
        var response = await _handler.Handle(new GetByIdAccountQuery(id), CancellationToken.None);

        // Assert
        Assert.Equal(id, response.Id);
        Assert.Equal("Nome", response.Name);
        Assert.Equal("Tipo", response.Type);
        Assert.Equal(123.45m, response.CurrentBalance);
        Assert.Equal(account.CreatedAt, response.CreatedAt);
    }

    [Fact]
    public async Task GetById_InvalidId_ThrowsNotFound()
    {
        // Arrange
        var id = Guid.NewGuid();
        var userId = Guid.NewGuid();
        _userContext.UserId.Returns(userId);
        _accountRepository.GetByIdAsync(id, userId, Arg.Any<CancellationToken>()).ReturnsNull();

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            _handler.Handle(new GetByIdAccountQuery(id), CancellationToken.None));
    }
}
