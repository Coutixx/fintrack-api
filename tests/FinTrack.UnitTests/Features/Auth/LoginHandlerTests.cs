using System.Security.Authentication;
using FinTrack.Application.Common.Interfaces;
using FinTrack.Application.Features.Auth;
using FinTrack.Domain.Entities;
using NSubstitute;

namespace FinTrack.UnitTests.Features.Auth.Login;

public class LoginHandlerTests
{
    private readonly IUserRepository _userRepository = Substitute.For<IUserRepository>();
    private readonly ITokenService _tokenService = Substitute.For<ITokenService>();
    private readonly IPasswordHasher _passwordHasher = Substitute.For<IPasswordHasher>();
    private readonly LoginHandler _handler;

    public LoginHandlerTests() =>
        _handler = new LoginHandler(_userRepository, _tokenService, _passwordHasher);

    [Fact]
    public async Task Handle_UserDoesNotExist_ShouldThrowInvalidCredentialException()
    {
        // Arrange
        var request = new LoginCommand("email@email.com", "password123");

        _userRepository
            .GetByEmailAsync(request.Email, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<User?>(null));

        // Assert
        await Assert.ThrowsAsync<InvalidCredentialException>(() =>
            _handler.Handle(request, CancellationToken.None)
    );
    }

    [Fact]
    public async Task Handle_ValidCredentials_ShouldReturnToken()
    {
        // Arrange
        var request = new LoginCommand("email@email.com", "password123");
        var user = new User
        {
            Email = "email@email.com",
            Name = "Coutinho",
            PasswordHash = "fake-hash"
        };

        _userRepository
            .GetByEmailAsync(request.Email, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<User?>(user));

        _passwordHasher
            .Verify(request.Password, user.PasswordHash)
            .Returns(true);

        _tokenService
            .GenerateToken(user)
            .Returns("fake-token");

        // Act
        var response = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.NotNull(response);
        Assert.Equal("fake-token", response.Token);
        Assert.Equal(user.Email, response.Email);
        Assert.Equal(user.Name, response.Name);
        _passwordHasher.Received(1).Verify(request.Password, user.PasswordHash);
        _tokenService.Received(1).GenerateToken(user);
    }

    [Fact]
    public async Task Handle_InvalidPassword_ShouldThrowInvalidCredentialException()
    {
        // Arrange
        var request = new LoginCommand("email@email.com", "password123");
        var user = new User { Email = "email@email.com", PasswordHash = "fake-hash" };

        _userRepository
            .GetByEmailAsync(request.Email, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<User?>(user));

        _passwordHasher
            .Verify(request.Password, Arg.Any<string>())
            .Returns(false);

        // Assert
        await Assert.ThrowsAsync<InvalidCredentialException>(() =>
            _handler.Handle(request, CancellationToken.None));

        _tokenService.DidNotReceive().GenerateToken(Arg.Any<User>());
    }
}
