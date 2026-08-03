using FinTrack.Application.Common.Interfaces;
using FinTrack.Application.Features.Accounts;
using FinTrack.Domain.Entities;
using NSubstitute;
using NSubstitute.ReturnsExtensions;


namespace FinTrack.UnitTests.Features.Accounts;

public class DeleteAccountHandlerTests
{
    private readonly IAccountRepository _accountRepository = Substitute.For<IAccountRepository>();
    private readonly IUserContext _userContext = Substitute.For<IUserContext>();
    private readonly DeleteAccountHandler _handler;
    public DeleteAccountHandlerTests() =>
        _handler = new DeleteAccountHandler(_accountRepository, _userContext);

    [Fact]
    public async Task Handle_ValidRequest_DeletesAccount()
    {
        // Arrange
        var id = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var existingAccount = new Account
        {
            Id = id,
            UserId = userId,
            Name = "Nome Antigo",
            Type = "Tipo Antigo"
        };

        _accountRepository
            .GetByIdAsync(id, userId, Arg.Any<CancellationToken>())
            .Returns(existingAccount);

        _userContext.UserId.Returns(userId);

        // Act
        await _handler.Handle(new DeleteAccountCommand(id), CancellationToken.None);

        // Assert
        Assert.NotNull(existingAccount.DeletedAt);
        Assert.InRange(existingAccount.DeletedAt.Value,
            DateTime.UtcNow.AddSeconds(-2),
            DateTime.UtcNow.AddSeconds(2));
    }

    [Fact]
    public async Task Handle_NonExistingAccount_ThrowsNotFound()
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
            _handler.Handle(new DeleteAccountCommand(id), CancellationToken.None));
    }
}
