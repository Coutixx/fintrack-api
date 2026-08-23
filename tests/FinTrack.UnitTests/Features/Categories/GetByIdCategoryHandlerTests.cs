using FinTrack.Application.Common.Interfaces;
using FinTrack.Application.Features.Categories;
using FinTrack.Domain.Entities;
using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace FinTrack.UnitTests.Features.Categories;

public class GetByIdCategoryHandlerTests
{
    private readonly ICategoryRepository _categoryRepository = Substitute.For<ICategoryRepository>();

    private readonly IUserContext _userContext = Substitute.For<IUserContext>();

    private readonly GetByIdCategoryHandler _handler;

    public GetByIdCategoryHandlerTests() =>
        _handler = new GetByIdCategoryHandler(_categoryRepository, _userContext);

    [Fact]
    public async Task GetById_ValidId_ReturnsCategory()
    {
        // Arrange
        var id = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var category = new Category
        {
            Id = id,
            UserId = userId,
            Name = "Nome",
            Type = "Tipo",
            Color = "Blue",
            CreatedAt = DateTime.UtcNow.AddDays(-1)
        };
        _userContext.UserId.Returns(userId);
        _categoryRepository.GetByIdAsync(id, userId, Arg.Any<CancellationToken>()).Returns(category);

        // Act
        var response = await _handler.Handle(new GetByIdCategoryQuery(id), CancellationToken.None);

        // Assert
        Assert.Equal(id, response.Id);
        Assert.Equal("Nome", response.Name);
        Assert.Equal("Tipo", response.Type);
        Assert.Equal("Blue", response.Color);
        Assert.Equal(category.CreatedAt, response.CreatedAt);
    }

    [Fact]
    public async Task GetById_InvalidId_ThrowsNotFound()
    {
        // Arrange
        var id = Guid.NewGuid();
        var userId = Guid.NewGuid();
        _userContext.UserId.Returns(userId);
        _categoryRepository.GetByIdAsync(id, userId, Arg.Any<CancellationToken>()).ReturnsNull();

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
        _handler.Handle(new GetByIdCategoryQuery(id), CancellationToken.None));
    }
}
