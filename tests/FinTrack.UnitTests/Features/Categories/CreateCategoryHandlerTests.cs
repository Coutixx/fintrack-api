using FinTrack.Application.Common.Interfaces;
using FinTrack.Application.Features.Categories;
using FinTrack.Domain.Entities;
using NSubstitute;

namespace FinTrack.UnitTests.Features.Categories;

public class CreateCategoryHandlerTests
{
    private readonly ICategoryRepository _categoryRepository = Substitute.For<ICategoryRepository>();

    private readonly IUserContext _userContext = Substitute.For<IUserContext>();

    private readonly CreateCategoryHandler _handler;

    public CreateCategoryHandlerTests() =>
        _handler = new CreateCategoryHandler(_categoryRepository, _userContext);

    [Fact]
    public async Task Handle_WhenCommandIsValid_ReturnsCategoryId()
    {
        // Arrange
        var request = new CreateCategoryCommand("Henrique", "Categoria", "Blue");
        _categoryRepository.AddAsync(Arg.Any<Category>()).Returns(Task.CompletedTask);

        // Act
        var response = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.NotNull(response);
        Assert.NotEqual(Guid.Empty, response.Id);
        await _categoryRepository.Received(1).AddAsync(Arg.Is<Category>(c =>
            c.Name == request.Name &&
            c.Type == request.Type &&
            c.Color == request.Color
        ));
    }

    [Fact]
    public async Task Handle_WhenCalled_UsesUserIdFromUserContext()
    {
        // Arrange
        var request = new CreateCategoryCommand("Henrique", "Categoria", "Blue");

        // Act
        await _handler.Handle(request, CancellationToken.None);

        // Assert
        await _categoryRepository.Received(1).AddAsync(Arg.Is<Category>(c => c.UserId == _userContext.UserId));
    }

    [Fact]
    public async Task Handle_WhenCommandIsValid_SetsCreatedAt()
    {
        // Arrange
        var request = new CreateCategoryCommand("Henrique", "Categoria", "Blue");
        var beforeExecution = DateTime.UtcNow;

        // Act
        await _handler.Handle(request, CancellationToken.None);

        // Assert
        await _categoryRepository.Received(1).AddAsync(Arg.Is<Category>(c =>
        c.CreatedAt >= beforeExecution &&
        c.CreatedAt <= DateTime.UtcNow
        ));
    }
}
