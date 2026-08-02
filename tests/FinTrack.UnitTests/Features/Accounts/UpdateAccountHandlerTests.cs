using FinTrack.Application.Common.Interfaces;
using FinTrack.Application.Features.Accounts;
using FinTrack.Domain.Entities;
using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace FinTrack.UnitTests.Features.Accounts;

public class UpdateAccountHandlerTests
{
    private readonly IAccountRepository _accountRepository = Substitute.For<IAccountRepository>();
    private readonly IUserContext _userContext = Substitute.For<IUserContext>();
    private readonly UpdateAccountHandler _handler;
    public UpdateAccountHandlerTests() =>
        _handler = new UpdateAccountHandler(_accountRepository, _userContext);

    [Fact]
    public async Task Handle_ValidRequest_UpdatesAccount()
    {
        // Arrange
        var id = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var existingAccount = new Account { Id = id, UserId = userId, Name = "Nome Antigo", Type = "Tipo Antigo" };

        _userContext.UserId.Returns(userId);

        _accountRepository
            .GetByIdAsync(id, userId, Arg.Any<CancellationToken>()).
            Returns(existingAccount);

        var request = new UpdateAccountCommand(id, "Nome Novo", "Tipo Novo");

        // Act
        var response = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.Equal("Nome Novo", response.Name);
        Assert.Equal("Tipo Novo", response.Type);

        await _accountRepository.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_NonExistingAccount_ThrowsNotFound()
    {
        // Arrange
        var id = Guid.NewGuid();
        var userId = Guid.NewGuid();

        _userContext.UserId.Returns(userId);
        _accountRepository
            .GetByIdAsync(id, userId, Arg.Any<CancellationToken>()).
            ReturnsNull();

        var request = new UpdateAccountCommand(id, "Nome novo", "Tipo Novo");

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
        _handler.Handle(request, CancellationToken.None));
    }
}
