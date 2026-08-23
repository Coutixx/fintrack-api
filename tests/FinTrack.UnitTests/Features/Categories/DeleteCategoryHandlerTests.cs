using FinTrack.Application.Common.Interfaces;
using FinTrack.Application.Features.Categories;
using FinTrack.Domain.Entities;
using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace FinTrack.UnitTests.Features.Categories;

public class DeleteCategoryHandlerTests
{
    private readonly ICategoryRepository _categoryRepository = Substitute.For<ICategoryRepository>();

    private readonly IUserContext _userContext = Substitute.For<IUserContext>();

    private readonly DeleteCategoryHandler _handler;

    public DeleteCategoryHandlerTests() =>
        _handler = new DeleteCategoryHandler(_categoryRepository, _userContext);

    [Fact]
    public async Task Handle_ValidRequest_DeletesCategory()
    {
        // Arrange
        var id = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var existingCategory = new Category
        {
            Id = id,
            UserId = userId,
            Name = "Nome",
            Type = "Tipo",
        };

        _categoryRepository
            .GetByIdAsync(id, userId, Arg.Any<CancellationToken>())
            .Returns(existingCategory);

        _userContext.UserId.Returns(userId);

        // Act
        await _handler.Handle(new DeleteCategoryCommand(id), CancellationToken.None);

        // Assert
        Assert.NotNull(existingCategory.DeletedAt);
        Assert.InRange(existingCategory.DeletedAt.Value,
            DateTime.UtcNow.AddSeconds(-2),
            DateTime.UtcNow.AddSeconds(2));
    }

    [Fact]
    public async Task Handle_NonExistingAccount_ThrowsNotFound()
    {
        // Arrange
        var id = Guid.NewGuid();
        var userId = Guid.NewGuid();

        _userContext.UserId.Returns(userId);

        _categoryRepository.
            GetByIdAsync(id, userId, Arg.Any<CancellationToken>()).
            ReturnsNull();

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            _handler.Handle(new DeleteCategoryCommand(id), CancellationToken.None));
    }
}
