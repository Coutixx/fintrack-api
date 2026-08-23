using FinTrack.Application.Common.Interfaces;
using FinTrack.Application.Features.Categories;
using FinTrack.Domain.Entities;
using NSubstitute;

namespace FinTrack.UnitTests.Features.Categories;

public class GetAllCategoriesHandlerTests
{
    private readonly ICategoryRepository _categoryRepository = Substitute.For<ICategoryRepository>();

    private readonly IUserContext _userContext = Substitute.For<IUserContext>();

    private readonly GetAllCategoriesHandler _handler;

    public GetAllCategoriesHandlerTests() =>
        _handler = new GetAllCategoriesHandler(_categoryRepository, _userContext);

    [Fact]
    public async Task GetAll_UserHasCategories_ReturnsList()
    {
        // Arrange
        var id = Guid.NewGuid();
        var type = "Tipo";
        var category = new Category { Name = "Conta", Type = type, UserId = id };
        var categories = new List<Category> { category };

        _userContext.UserId.Returns(id);
        _categoryRepository.GetAllAsync(id, type, Arg.Any<CancellationToken>()).Returns(categories);

        // Act
        var response = await _handler.Handle(new GetAllCategoriesQuery(type), CancellationToken.None);

        // Assert
        Assert.NotNull(response);
        Assert.Single(response.categories);
        Assert.Equal("Conta", response.categories.First().Name);

    }

    [Fact]
    public async Task GetAll_UserHasNoCategories_ReturnsEmptyList()
    {
        // Arrange
        var id = Guid.NewGuid();
        var type = "Tipo";
        _userContext.UserId.Returns(id);
        _categoryRepository.GetAllAsync(id, type, Arg.Any<CancellationToken>()).Returns(new List<Category>());

        // Act
        var response = await _handler.Handle(new GetAllCategoriesQuery(type), CancellationToken.None);

        // Assert
        Assert.NotNull(response);
        Assert.Empty(response.categories);
    }

    [Fact]
    public async Task GetAll_ValidRequest_FiltersByUserId()
    {
        // Arrange
        var id = Guid.NewGuid();
        _userContext.UserId.Returns(id);

        // Act
        await _handler.Handle(new GetAllCategoriesQuery(), CancellationToken.None);

        // Assert
        await _categoryRepository.Received(1).GetAllAsync(id, null, CancellationToken.None);
    }

    [Fact]
    public async Task GetAll_WhenTypeIsInformed_PassesTypeToRepository()
    {
        // Arrange
        var id = Guid.NewGuid();
        var type = "Tipo";
        _userContext.UserId.Returns(id);

        // Act
        await _handler.Handle(new GetAllCategoriesQuery(type), CancellationToken.None);

        // Assert
        await _categoryRepository.Received(1).GetAllAsync(id, type, CancellationToken.None);
    }
}
