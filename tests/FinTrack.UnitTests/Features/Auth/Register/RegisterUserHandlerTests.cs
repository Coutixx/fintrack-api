using FinTrack.Application.Common.Interfaces;
using FinTrack.Application.Features.Auth.Register;
using FinTrack.Domain.Entities;

using NSubstitute;

namespace FinTrack.UnitTests.Features.Auth.Register;

public class RegisterUserHandlerTests
{
    private readonly IUserRepository _userRepositoryMock;
    private readonly ITokenService _tokenServiceMock;
    private readonly IPasswordHasher _passwordHasherMock;
    private readonly RegisterUserHandler _handler;

    public RegisterUserHandlerTests()
    {
        _userRepositoryMock = Substitute.For<IUserRepository>();
        _tokenServiceMock = Substitute.For<ITokenService>();
        _passwordHasherMock = Substitute.For<IPasswordHasher>();

        _handler = new RegisterUserHandler(_userRepositoryMock, _tokenServiceMock, _passwordHasherMock);
    }

    [Fact]
    public async Task Handle_ValidRequest_ShouldCreateUser()
    {
        var request = new RegisterUserCommand("Coutinho", "email@email.com", "password123");

        _userRepositoryMock
            .ExistingByEmailAsync(request.Email, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(false));

        _userRepositoryMock
            .AddAsync(Arg.Any<User>())
            .Returns(Task.CompletedTask);

        await _handler.Handle(request, CancellationToken.None);

        await _userRepositoryMock.Received(1).AddAsync(Arg.Is<User>(u =>
            u.Name == request.Name &&
            u.Email == request.Email));
    }

    [Fact]
    public async Task Handle_ValidRequest_ShouldCreateUser_AndHashPassword()
    {
        var request = new RegisterUserCommand("Coutinho", "email@email.com", "password123");

        _userRepositoryMock
            .ExistingByEmailAsync(request.Email, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(false));

        _passwordHasherMock
                    .Hash(request.Password).Returns("fake-hash");

        await _handler.Handle(request, CancellationToken.None);

        _passwordHasherMock.Received(1).Hash(request.Password);

        await _userRepositoryMock.Received(1).AddAsync(Arg.Is<User>(u =>
            u.Name == request.Name &&
            u.Email == request.Email &&
            u.PasswordHash == "fake-hash"));
    }
}
