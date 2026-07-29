using FinTrack.Application.Common.Interfaces;
using FinTrack.Application.Features.Auth;
using FinTrack.Domain.Entities;
using NSubstitute;

namespace FinTrack.UnitTests.Features.Auth.Register;

public class HandlerTests
{
    private readonly IUserRepository _userRepository = Substitute.For<IUserRepository>();
    private readonly ITokenService _tokenService = Substitute.For<ITokenService>();
    private readonly IPasswordHasher _passwordHasher = Substitute.For<IPasswordHasher>();
    private readonly RegisterHandler _handler;

    public HandlerTests() =>
        _handler = new RegisterHandler(_userRepository, _tokenService, _passwordHasher);

    [Fact]
    public async Task Handle_ValidRequest_ShouldCreateUser()
    {
        var request = new RegisterCommand("Coutinho", "email@email.com", "password123");

        _userRepository
            .ExistingByEmailAsync(request.Email, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(false));

        _userRepository
            .AddAsync(Arg.Any<User>())
            .Returns(Task.CompletedTask);

        var response = await _handler.Handle(request, CancellationToken.None);
        Assert.NotNull(response);
        Assert.NotEqual(Guid.Empty, response.Id);

        await _userRepository.Received(1).AddAsync(Arg.Is<User>(u =>
            u.Name == request.Name &&
            u.Email == request.Email
        ));
    }

    [Fact]
    public async Task Handle_ValidRequest_ShouldCreateUser_AndHashPassword()
    {
        var request = new RegisterCommand("Coutinho", "email@email.com", "password123");

        _userRepository
            .ExistingByEmailAsync(request.Email, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(false));

        _passwordHasher
                    .Hash(request.Password).Returns("fake-hash");

        await _handler.Handle(request, CancellationToken.None);

        _passwordHasher.Received(1).Hash(request.Password);

        await _userRepository.Received(1).AddAsync(Arg.Is<User>(u =>
            u.Name == request.Name &&
            u.Email == request.Email &&
            u.PasswordHash == "fake-hash"));
    }
}
