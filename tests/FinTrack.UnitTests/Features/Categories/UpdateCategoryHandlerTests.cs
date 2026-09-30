using FinTrack.Application.Common.Interfaces;
using FinTrack.Application.Features.Categories;
using FinTrack.Domain.Entities;
using FinTrack.Domain.Enums;
using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace FinTrack.UnitTests.Features.Categories;

public class UpdateCategoryHandlerTests
{
    private readonly ICategoryRepository _categoryRepository = Substitute.For<ICategoryRepository>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();

    private readonly IUserContext _userContext = Substitute.For<IUserContext>();

    private readonly UpdateCategoryHandler _handler;

    public UpdateCategoryHandlerTests() =>
        _handler = new UpdateCategoryHandler(_categoryRepository, _unitOfWork, _userContext);

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
            Type = TransactionType.Income,
            Color = "Cor Antiga"
        };
        _userContext.UserId.Returns(userId);
        _categoryRepository.GetByIdAsync(id, userId, CancellationToken.None).Returns(existingCategory);
        // Act
        var response = await _handler.Handle(new UpdateCategoryCommand(
            id,
            "Nome Novo",
            TransactionType.Income,
            "Cor Nova"
            ), CancellationToken.None);

        // Assert
        Assert.Equal("Nome Novo", response.Name);
        Assert.Equal(TransactionType.Income, response.Type);
        Assert.Equal("Cor Nova", response.Color);
        Assert.Equal(id, response.Id);
        await _categoryRepository.DidNotReceive().HasTransactionsAsync(id, CancellationToken.None);
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenChangingTypeWithLinkedTransactions_ThrowsAndDoesNotSave()
    {
        var id = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var existingCategory = new Category
        {
            Id = id,
            UserId = userId,
            Name = "Nome Antigo",
            Type = TransactionType.Income,
            Color = "Cor Antiga"
        };
        _userContext.UserId.Returns(userId);
        _categoryRepository.GetByIdAsync(id, userId, CancellationToken.None).Returns(existingCategory);
        _categoryRepository.HasTransactionsAsync(id, CancellationToken.None).Returns(true);

        var exception = await Assert.ThrowsAsync<ArgumentException>(() =>
            _handler.Handle(
                new UpdateCategoryCommand(id, "Nome Novo", TransactionType.Expense, "Cor Nova"),
                CancellationToken.None));

        Assert.Equal(
            "Não é possível alterar o tipo da categoria porque existem transações vinculadas.",
            exception.Message);
        Assert.Equal("Nome Antigo", existingCategory.Name);
        Assert.Equal(TransactionType.Income, existingCategory.Type);
        Assert.Equal("Cor Antiga", existingCategory.Color);
        await _categoryRepository.Received(1).HasTransactionsAsync(id, CancellationToken.None);
        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenChangingTypeWithoutTransactions_UpdatesCategory()
    {
        var id = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var existingCategory = new Category
        {
            Id = id,
            UserId = userId,
            Name = "Nome",
            Type = TransactionType.Income,
            Color = "Azul"
        };
        _userContext.UserId.Returns(userId);
        _categoryRepository.GetByIdAsync(id, userId, CancellationToken.None).Returns(existingCategory);
        _categoryRepository.HasTransactionsAsync(id, CancellationToken.None).Returns(false);

        var response = await _handler.Handle(
            new UpdateCategoryCommand(id, "Nome", TransactionType.Expense, "Vermelho"),
            CancellationToken.None);

        Assert.Equal(TransactionType.Expense, response.Type);
        Assert.Equal("Vermelho", response.Color);
        await _categoryRepository.Received(1).HasTransactionsAsync(id, CancellationToken.None);
        await _unitOfWork.Received(1).SaveChangesAsync(CancellationToken.None);
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
            TransactionType.Income,
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
            Type = TransactionType.Income,
            Color = "Cor Antiga"
        };
        _userContext.UserId.Returns(userId);
        _categoryRepository.GetByIdAsync(id, userId, CancellationToken.None).Returns(existingCategory);

        // Act
        await _handler.Handle(new UpdateCategoryCommand(
            id,
            "Nome Novo",
            TransactionType.Income,
            "Cor Nova"
            ), CancellationToken.None);

        // Assert
        Assert.NotNull(existingCategory.UpdatedAt);
        Assert.InRange(existingCategory.UpdatedAt.Value,
            DateTime.UtcNow.AddSeconds(-2),
            DateTime.UtcNow.AddSeconds(2));
    }
}
