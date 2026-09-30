using FinTrack.Application.Common.Interfaces;
using FinTrack.Application.Common.Models;
using FinTrack.Application.Features.Categories;
using FinTrack.Domain.Enums;
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
        var userId = Guid.NewGuid();
        var type = TransactionType.Income;
        var category = new CategoryReadModel(
            id,
            "Conta",
            type,
            "Preto"
        );
        var categories = new List<CategoryReadModel> { category };

        _userContext.UserId.Returns(id);
        _categoryRepository.GetAllAsync(id, type, 1, 10, Arg.Any<CancellationToken>())
            .Returns(new CategoryPageReadModel(categories, 1));

        // Act
        var response = await _handler.Handle(new GetAllCategoriesQuery(type), CancellationToken.None);

        // Assert
        Assert.NotNull(response);
        Assert.Single(response.Categories);
        Assert.Equal("Conta", response.Categories.First().Name);
        Assert.Equal(1, response.Page);
        Assert.Equal(10, response.PageSize);
        Assert.Equal(1, response.TotalCount);

    }

    [Fact]
    public async Task GetAll_UserHasNoCategories_ReturnsEmptyList()
    {
        // Arrange
        var id = Guid.NewGuid();

        var type = TransactionType.Income;
        _userContext.UserId.Returns(id);
        _categoryRepository.GetAllAsync(id, type, 1, 10, Arg.Any<CancellationToken>())
            .Returns(new CategoryPageReadModel(new List<CategoryReadModel>(), 0));

        // Act
        var response = await _handler.Handle(new GetAllCategoriesQuery(type), CancellationToken.None);

        // Assert
        Assert.NotNull(response);
        Assert.Empty(response.Categories);
    }

    [Fact]
    public async Task GetAll_ValidRequest_FiltersByUserId()
    {
        // Arrange
        var id = Guid.NewGuid();
        _userContext.UserId.Returns(id);
        _categoryRepository.GetAllAsync(id, null, 1, 10, CancellationToken.None)
            .Returns(new CategoryPageReadModel(new List<CategoryReadModel>(), 0));

        // Act
        await _handler.Handle(new GetAllCategoriesQuery(), CancellationToken.None);

        // Assert
        await _categoryRepository.Received(1).GetAllAsync(id, null, 1, 10, CancellationToken.None);
    }

    [Fact]
    public async Task GetAll_WhenTypeIsInformed_PassesTypeToRepository()
    {
        // Arrange
        var id = Guid.NewGuid();
        var type = TransactionType.Income;
        _userContext.UserId.Returns(id);
        _categoryRepository.GetAllAsync(id, type, 1, 10, CancellationToken.None)
            .Returns(new CategoryPageReadModel(new List<CategoryReadModel>(), 0));

        // Act
        await _handler.Handle(new GetAllCategoriesQuery(type), CancellationToken.None);

        // Assert
        await _categoryRepository.Received(1).GetAllAsync(id, type, 1, 10, CancellationToken.None);
    }

    [Fact]
    public async Task GetAll_WhenPageIsInformed_PassesPageToRepository()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var type = TransactionType.Expense;
        _userContext.UserId.Returns(userId);
        _categoryRepository.GetAllAsync(userId, type, 2, 5, CancellationToken.None)
            .Returns(new CategoryPageReadModel(new List<CategoryReadModel>(), 12));

        // Act
        var response = await _handler.Handle(new GetAllCategoriesQuery(type, 2, 5), CancellationToken.None);

        // Assert
        Assert.Equal(2, response.Page);
        Assert.Equal(5, response.PageSize);
        Assert.Equal(12, response.TotalCount);
        await _categoryRepository.Received(1).GetAllAsync(userId, type, 2, 5, CancellationToken.None);
    }
}
