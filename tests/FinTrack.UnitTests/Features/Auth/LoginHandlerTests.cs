using System.Security.Authentication;
using FinTrack.Application.Common.Interfaces;
using FinTrack.Application.Features.Auth;
using FinTrack.Domain.Entities;

using NSubstitute;

namespace FinTrack.UnitTests.Features.Auth.Login;

public class HandlerTests
{
    private readonly IUserRepository _userRepository = Substitute.For<IUserRepository>();
    private readonly ITokenService _tokenService = Substitute.For<ITokenService>();
    private readonly IPasswordHasher _passwordHasher = Substitute.For<IPasswordHasher>();
    private readonly LoginHandler _handler;

    public HandlerTests() =>
        _handler = new LoginHandler(_userRepository, _tokenService, _passwordHasher);

    [Fact]
    public async Task Handle_UserDoesNotExist_ShouldThrowInvalidCredentialException()
    {
        var request = new LoginCommand("email@email.com", "password123");

        _userRepository
            .GetByEmailAsync(request.Email, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<User?>(null));

        await Assert.ThrowsAsync<InvalidCredentialException>(() =>
            _handler.Handle(request, CancellationToken.None)
    );
    }

    [Fact]
    public async Task Handle_ValidCredentials_ShouldReturnToken()
    {
        var request = new LoginCommand("email@email.com", "password123");

        var user = new User { Email = "email@email.com" };

        _userRepository
            .GetByEmailAsync(request.Email, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<User?>(user));

        _passwordHasher
            .Verify(request.Password, Arg.Any<string>())
            .Returns(true);

        _tokenService
            .GenerateToken(user)
            .Returns("fake-token");

        var result = await _handler.Handle(request, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal("fake-token", result.Token);
    }

    [Fact]
    public async Task Handle_InvalidPassword_ShouldThrowInvalidCredentialException()
    {
        var request = new LoginCommand("email@email.com", "password123");

        var user = new User { Email = "email@email.com", PasswordHash = "fake-hash" };

        _userRepository
            .GetByEmailAsync(request.Email, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<User?>(user));

        _passwordHasher
            .Verify(request.Password, Arg.Any<string>())
            .Returns(false);

        await Assert.ThrowsAsync<InvalidCredentialException>(() =>
            _handler.Handle(request, CancellationToken.None));
    }
}
