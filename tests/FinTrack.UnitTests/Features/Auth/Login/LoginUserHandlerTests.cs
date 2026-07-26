using System.Security.Authentication;

using FinTrack.Application.Common.Interfaces;
using FinTrack.Application.Features.Auth.Login;
using FinTrack.Domain.Entities;

using NSubstitute;

namespace FinTrack.UnitTests.Features.Auth.Login;

public class LoginUserHandlerTests
{
    private readonly IUserRepository _userRepositoryMock;
    private readonly ITokenService _tokenServiceMock;
    private readonly IPasswordHasher _passwordHasherMock;
    private readonly LoginUserHandler _handler;

    public LoginUserHandlerTests()
    {
        _userRepositoryMock = Substitute.For<IUserRepository>();
        _tokenServiceMock = Substitute.For<ITokenService>();
        _passwordHasherMock = Substitute.For<IPasswordHasher>();

        _handler = new LoginUserHandler(_userRepositoryMock, _tokenServiceMock, _passwordHasherMock);
    }

    [Fact]
    public async Task Handle_UserDoesNotExist_ShouldThrowInvalidCredentialException()
    {
        var request = new LoginUserCommand("email@email.com", "password123");

        _userRepositoryMock
            .GetByEmailAsync(request.Email, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<User?>(null));

        await Assert.ThrowsAsync<InvalidCredentialException>(() =>
            _handler.Handle(request, CancellationToken.None)
    );
    }

    [Fact]
    public async Task Handle_ValidCredentials_ShouldReturnToken()
    {
        var request = new LoginUserCommand("email@email.com", "password123");

        var user = new User { Email = "email@email.com" };

        _userRepositoryMock
            .GetByEmailAsync(request.Email, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<User?>(user));

        _passwordHasherMock
            .Verify(request.Password, Arg.Any<string>())
            .Returns(true);

        _tokenServiceMock
            .GenerateToken(user)
            .Returns("fake-token");

        var result = await _handler.Handle(request, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal("fake-token", result.Token);
    }

    [Fact]
    public async Task Handle_InvalidPassword_ShouldThrowInvalidCredentialException()
    {
        var request = new LoginUserCommand("email@email.com", "password123");

        var user = new User { Email = "email@email.com", PasswordHash = "fake-hash" };

        _userRepositoryMock
            .GetByEmailAsync(request.Email, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<User?>(user));

        _passwordHasherMock
            .Verify(request.Password, Arg.Any<string>())
            .Returns(false);

        await Assert.ThrowsAsync<InvalidCredentialException>(() =>
            _handler.Handle(request, CancellationToken.None));
    }


}
