using FinTrack.Application.Common.Interfaces;
using FinTrack.Application.Features.Categories;
using FinTrack.Domain.Entities;
using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace FinTrack.UnitTests.Features.Categories;

public class UpdateCategoryHandlerTests
{
    private readonly ICategoryRepository _categoryRepository = Substitute.For<ICategoryRepository>();

    private readonly IUserContext _userContext = Substitute.For<IUserContext>();

    private readonly UpdateCategoryHandler _handler;

    public UpdateCategoryHandlerTests() =>
        _handler = new UpdateCategoryHandler(_categoryRepository, _userContext);

    [Fact]
    public async Task Handle_ValidRequest_UpdatesCategory()
    {
        // Arrange
        var id = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var existingCategory = new Category
        {
            Id = id,
            UserId = userId,
            Name = "Nome Antigo",
            Type = "Tipo Antigo",
            Color = "Cor Antiga"
        };
        _userContext.UserId.Returns(userId);
        _categoryRepository.GetByIdAsync(id, userId, CancellationToken.None).Returns(existingCategory);

        // Act
        var response = await _handler.Handle(new UpdateCategoryCommand(
            id,
            "Nome Novo",
            "Tipo Novo",
            "Cor Nova"
            ), CancellationToken.None);

        // Assert
        Assert.Equal("Nome Novo", response.Name);
        Assert.Equal("Tipo Novo", response.Type);
        Assert.Equal("Cor Nova", response.Color);
        Assert.Equal(id, response.Id);
        Assert.NotNull(response.UpdatedAt);
        await _categoryRepository.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_NonExistingCategory_ThrowsNotFound()
    {
        // Arrange
        var id = Guid.NewGuid();
        var userId = Guid.NewGuid();
        _userContext.UserId.Returns(userId);
        _categoryRepository.GetByIdAsync(id, userId, Arg.Any<CancellationToken>()).ReturnsNull();

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            _handler.Handle(new UpdateCategoryCommand(
            id,
            "Nome Novo",
            "Tipo Novo",
            "Cor Nova"
            ), CancellationToken.None));
    }

    [Fact]
    public async Task Handle_ValidRequest_SetsUpdatedAt()
    {
        // Arrange
        var id = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var existingCategory = new Category
        {
            Id = id,
            UserId = userId,
            Name = "Nome Antigo",
            Type = "Tipo Antigo",
            Color = "Cor Antiga"
        };
        _userContext.UserId.Returns(userId);
        _categoryRepository.GetByIdAsync(id, userId, CancellationToken.None).Returns(existingCategory);

        // Act
        await _handler.Handle(new UpdateCategoryCommand(
            id,
            "Nome Novo",
            "Tipo Novo",
            "Cor Nova"
            ), CancellationToken.None);

        // Assert
        Assert.NotNull(existingCategory.UpdatedAt);
        Assert.InRange(existingCategory.UpdatedAt.Value,
            DateTime.UtcNow.AddSeconds(-2),
            DateTime.UtcNow.AddSeconds(2));
    }
}
