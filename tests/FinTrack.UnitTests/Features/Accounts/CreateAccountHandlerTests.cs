using FinTrack.Application.Common.Interfaces;
using FinTrack.Application.Features.Accounts;
using FinTrack.Domain.Entities;
using FinTrack.Domain.Enums;
using NSubstitute;

namespace FinTrack.UnitTests.Features.Accounts;

public class CreateAccountHandlerTests
{
    private readonly IAccountRepository _accountRepository = Substitute.For<IAccountRepository>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly IUserContext _userContext = Substitute.For<IUserContext>();
    private readonly CreateAccountHandler _handler;

    public CreateAccountHandlerTests() =>
        _handler = new CreateAccountHandler(_accountRepository, _unitOfWork, _userContext);

    [Fact]
    public async Task Handle_WhenCommandIsValid_ReturnsAccountId()
    {
        // Arrange
        var request = new CreateAccountCommand("Henrique", AccountType.Checking, 1983.93m);
        var userId = Guid.NewGuid();
        _userContext.UserId.Returns(userId);
        _accountRepository.AddAsync(Arg.Any<Account>(), CancellationToken.None).Returns(Task.CompletedTask);

        // Act
        var response = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.NotNull(response);
        Assert.NotEqual(Guid.Empty, response.Id);
        await _accountRepository.Received(1).AddAsync(Arg.Is<Account>(a =>
            a.Name == request.Name &&
            a.Type == request.Type &&
            a.InitialBalance == request.InitialBalance &&
            a.CurrentBalance == request.InitialBalance &&
            a.UserId == userId &&
            a.CreatedAt != default
        ), CancellationToken.None);
        await _unitOfWork.Received(1).SaveChangesAsync(CancellationToken.None);
    }

    [Fact]
    public async Task Handle_WhenInitialBalanceIsNull_SetsBalanceToZero()
    {
        // Arrange
        var request = new CreateAccountCommand("Henrique", AccountType.Checking, null);
        var userId = Guid.NewGuid();
        _userContext.UserId.Returns(userId);

        // Act
        await _handler.Handle(request, CancellationToken.None);

        // Assert
        await _accountRepository.Received(1).AddAsync(Arg.Is<Account>(a => a.InitialBalance == 0), CancellationToken.None);
    }

    [Fact]
    public async Task Handle_WhenCalled_UsesUserIdFromUserContext()
    {
        // Arrange
        var request = new CreateAccountCommand("Henrique", AccountType.Checking, null);
        var userId = Guid.NewGuid();
        _userContext.UserId.Returns(userId);

        // Act
        await _handler.Handle(request, CancellationToken.None);

        // Assert
        await _accountRepository.Received(1).AddAsync(Arg.Is<Account>(a => a.UserId == userId), CancellationToken.None);
    }

    [Fact]
    public async Task Handle_WhenCommmandIsValid_SetsInitalAndCurrentBalanceEqually()
    {
        // Arrange
        var request = new CreateAccountCommand("Henrique", AccountType.Checking, 350.49m);

        // Act
        await _handler.Handle(request, CancellationToken.None);

        // Assert
        await _accountRepository.Received(1).AddAsync(Arg.Is<Account>(a => a.InitialBalance == request.InitialBalance &&
        a.CurrentBalance == request.InitialBalance), CancellationToken.None);
    }
}
