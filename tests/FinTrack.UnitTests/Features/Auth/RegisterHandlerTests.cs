using FinTrack.Application.Common.Interfaces;
using FinTrack.Application.Features.Auth;
using FinTrack.Domain.Entities;
using NSubstitute;

namespace FinTrack.UnitTests.Features.Auth.Register;

public class RegisterHandlerTests
{
    private readonly IUserRepository _userRepository = Substitute.For<IUserRepository>();
    private readonly ITokenService _tokenService = Substitute.For<ITokenService>();
    private readonly IPasswordHasher _passwordHasher = Substitute.For<IPasswordHasher>();
    private readonly RegisterHandler _handler;

    public RegisterHandlerTests() =>
        _handler = new RegisterHandler(_userRepository, _tokenService, _passwordHasher);

    [Fact]
    public async Task Handle_ValidRequest_ShouldCreateUser()
    {
        // Arrange
        var request = new RegisterCommand("Coutinho", "email@email.com", "password123");

        _userRepository
            .ExistingByEmailAsync(request.Email, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(false));

        _userRepository
            .AddAsync(Arg.Any<User>())
            .Returns(Task.CompletedTask);

        // Act
        var response = await _handler.Handle(request, CancellationToken.None);
        Assert.NotNull(response);
        Assert.NotEqual(Guid.Empty, response.Id);

        // Assert
        await _userRepository.Received(1).AddAsync(Arg.Is<User>(u =>
            u.Name == request.Name &&
            u.Email == request.Email
        ));
    }

    [Fact]
    public async Task Handle_ValidRequest_ShouldCreateUser_AndHashPassword()
    {
        // Arrange
        var request = new RegisterCommand("Coutinho", "email@email.com", "password123");

        _userRepository
            .ExistingByEmailAsync(request.Email, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(false));

        _passwordHasher
                    .Hash(request.Password).Returns("fake-hash");

        // Act
        await _handler.Handle(request, CancellationToken.None);

        // Assert
        _passwordHasher.Received(1).Hash(request.Password);

        await _userRepository.Received(1).AddAsync(Arg.Is<User>(u =>
            u.Name == request.Name &&
            u.Email == request.Email &&
            u.PasswordHash == "fake-hash"));
    }
}
