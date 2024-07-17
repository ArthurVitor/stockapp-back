using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Moq;
using StockApp.API.Repositories.Interfaces;
using StockApp.API.Services;
using StockApp.Models.Dtos.OverrideDto;
using StockApp.Models.Dtos.Pagination;
using StockApp.Models.Enum;
using StockApp.Models.Models;

namespace StockApp.Tests.Services;

[TestFixture]
public class OverrideServiceTests
{
    private Mock<IOverrideRepository> _overrideRepositoryMock;
    private Mock<ITransactionsRepository> _transactionsRepositoryMock;
    private Mock<IMapper> _mapperMock;
    private OverrideService _overrideService;

    [SetUp]
    public void SetUp()
    {
        _overrideRepositoryMock = new Mock<IOverrideRepository>();
        _transactionsRepositoryMock = new Mock<ITransactionsRepository>();
        _mapperMock = new Mock<IMapper>();

        _overrideService = new OverrideService(_overrideRepositoryMock.Object, _transactionsRepositoryMock.Object, _mapperMock.Object);
    }

    [Test]
    public async Task CreateOverrideAsync_ShouldReturnCreatedOverride_ForEntryNote_WhenSuccessful()
    {
        // Arrange
        var note = new EntryNote { Id = 1, Product = new Product { Id = 1 }, Quantity = 5 };
        var transaction = new Transactions { Id = 1, InventoryId = 1 };
        string expectedDescription = "The inventory with id 1 is 5 units surpassed the allowed amount for the product with id 1";

        //Config mock
        _transactionsRepositoryMock
            .Setup(repo => repo.GetTransactionByNoteIdAndNoteType(note.Id, TransactionTypeEnum.Entry))
            .Returns(transaction);

        // Act
        var result = await _overrideService.CreateOverrideAsync(note, "surpassed", 5);

        // Assert
        _overrideRepositoryMock.Verify(repo => repo.CreateOverrideAsync(It.Is<Override>(
            o => o.TransactionsId == transaction.Id && o.Description == expectedDescription)), Times.Once);

        Assert.Multiple(() =>
        {
            Assert.That(transaction.Id, Is.EqualTo(result.TransactionsId));
            Assert.That(expectedDescription, Is.EqualTo(result.Description));
        });
    }

    [Test]
    public async Task CreateOverrideAsync_ShouldReturnCreatedOverride_ForExitNote_WhenSuccessful()
    {
        // Arrange
        var note = new ExitNote { Id = 1, Product = new Product { Id = 1 }, Quantity = 5 };
        var transaction = new Transactions { Id = 1, InventoryId = 1 };
        string expectedDescription = "The inventory with id 1 is 5 units surpassed the allowed amount for the product with id 1";

        //Config mock
        _transactionsRepositoryMock
            .Setup(repo => repo.GetTransactionByNoteIdAndNoteType(note.Id, TransactionTypeEnum.Exit))
            .Returns(transaction);

        // Act
        var result = await _overrideService.CreateOverrideAsync(note, "surpassed", 5);

        // Assert
        _overrideRepositoryMock.Verify(repo => repo.CreateOverrideAsync(It.Is<Override>(
            o => o.TransactionsId == transaction.Id && o.Description == expectedDescription)), Times.Once);

        Assert.Multiple(() =>
        {
            Assert.That(transaction.Id, Is.EqualTo(result.TransactionsId));
            Assert.That(expectedDescription, Is.EqualTo(result.Description));
        });
    }

    [Test]
    public async Task GetAllOverridesAsync_ShouldReturnAllOverrides()
    {
        // Arrange
        var overrides = new List<Override> { new() { Id = 1 }, new() { Id = 2 } };
        int skip = 0;
        int take = 10;
        string orderBy = "id"; 
        bool isAscending = false;
        var overridesDto = new List<ListOverrideDto> { new() { Id = 1 }, new() { Id = 2 } };

        //config mock
        _overrideRepositoryMock.Setup(repo => repo.GetAllOverridesAsync(skip, take, orderBy, isAscending)).ReturnsAsync((overrides.Count,overrides));
        _mapperMock.Setup(m => m.Map<IEnumerable<ListOverrideDto>>(overrides)).Returns(overridesDto);

        // Act
        var result = await _overrideService.GetAllOverridesAsync(skip, take, orderBy, isAscending) as OkObjectResult;

        // Assert
        Assert.That(result, Is.Not.Null);
        var pagedResult = result.Value as PagedResultDto<ListOverrideDto>; 
        Assert.That(pagedResult, Is.Not.Null);
        Assert.That(pagedResult.Records, Is.EqualTo(overridesDto));
    }

    [Test]
    public async Task GetOverrideByIdAsync_ShouldReturnOverride_WhenOverrideExists()
    {
        // Arrange
        var overrideObject = new Override { Id = 1 };
        var overrideDto = new ListOverrideDto { Id = overrideObject.Id };

        //Config mock
        _overrideRepositoryMock.Setup(repo => repo.GetOverrideByIdAsync(overrideObject.Id)).ReturnsAsync(overrideObject);
        _mapperMock.Setup(m => m.Map<ListOverrideDto>(overrideObject)).Returns(overrideDto);

        // Act
        var result = await _overrideService.GetOverrideByIdAsync(overrideObject.Id) as OkObjectResult;

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(overrideDto, Is.EqualTo(result.Value));
    }

    [Test]
    public async Task GetOverrideByIdAsync_ShouldReturnNotFound_WhenOverrideDoesNotExist()
    {
        // Config mock
        _overrideRepositoryMock.Setup(repo => repo.GetOverrideByIdAsync(1)).ReturnsAsync((Override?)null);

        // Act
        var result = await _overrideService.GetOverrideByIdAsync(1);

        // Assert
        Assert.That(result, Is.InstanceOf<NotFoundResult>());
    }

    [Test]
    public async Task ValidateOverrideEntryNoteAsync_ShouldCreateOverride_WhenExceedsMaximumAmount()
    {
        // Arrange
        var note = new EntryNote { Id = 1, Product = new Product { Id = 1 }, Quantity = 10 };
        var parameters = new Parameters { MaximumAmount = 15 };
        double totalAvailableQuantity = 10 + note.Quantity; // the batch has already been created and its value has already updated the total quantity available at that point
        var transaction = new Transactions { Id = 1, InventoryId = 1 };

        //Config mock
        _transactionsRepositoryMock
            .Setup(repo => repo.GetTransactionByNoteIdAndNoteType(note.Id, TransactionTypeEnum.Entry))
            .Returns(transaction);

        // Act
        await _overrideService.ValidateOverrideAsync(note, totalAvailableQuantity, parameters);

        // Assert
        _overrideRepositoryMock.Verify(repo => repo.CreateOverrideAsync(It.IsAny<Override>()), Times.Once);
    }

    [Test]
    public async Task ValidateOverrideExitNoteAsync_ShouldCreateOverride_WhenBelowMinimumAmount()
    {
        // Arrange
        var note = new ExitNote { Id = 1, Product = new Product { Id = 1 }, Quantity = 20 };
        var parameters = new Parameters { MinimumAmount = 5 };
        double totalAvailableQuantity = 20 - note.Quantity; // the amount has already been deducted from inventory and its value has already updated the total quantity available at that point
        var transaction = new Transactions { Id = 1, InventoryId = 1 };

        //Config mock
        _transactionsRepositoryMock
            .Setup(repo => repo.GetTransactionByNoteIdAndNoteType(note.Id, TransactionTypeEnum.Exit))
            .Returns(transaction);

        // Act
        await _overrideService.ValidateOverrideAsync(note, totalAvailableQuantity, parameters);

        // Assert
        _overrideRepositoryMock.Verify(repo => repo.CreateOverrideAsync(It.IsAny<Override>()), Times.Once);
    }

    [Test]
    [TestCase(26, 30, 15, false)]
    [TestCase(50, 15, 45, true)]
    [TestCase(10, 20, 15, false)]
    public async Task ValidateOverrideAsync_ShouldHandleDifferentValues_EntryNote(
           double totalAvailableQuantity, 
           double paramLimit, double paramThreshold, bool shouldCreateOverride)
    {
        // Arrange
        var note = new EntryNote { Id = 1, Product = new Product { Id = 1 }};
        var parameters = new Parameters { MaximumAmount = paramLimit, MinimumAmount = paramThreshold };
        var transaction = new Transactions { Id = 1, InventoryId = 1 };

        //Config mock
        _transactionsRepositoryMock
            .Setup(repo => repo.GetTransactionByNoteIdAndNoteType(note.Id, TransactionTypeEnum.Entry))
            .Returns(transaction);

        if (shouldCreateOverride)
            _overrideRepositoryMock
                .Setup(repo => repo.CreateOverrideAsync(It.IsAny<Override>()))
                .Returns(Task.CompletedTask)
                .Verifiable();

        // Act
        await _overrideService.ValidateOverrideAsync(note, totalAvailableQuantity, parameters);

        // Assert
        if (shouldCreateOverride)
            _overrideRepositoryMock.Verify(repo => repo.CreateOverrideAsync(It.IsAny<Override>()), Times.Once);
        else
            _overrideRepositoryMock.Verify(repo => repo.CreateOverrideAsync(It.IsAny<Override>()), Times.Never);
    }

    [Test]
    [TestCase(10, 20, 15, true)]
    [TestCase(10, 2, 5, false)]
    [TestCase(20, 30, 15, false)]
    public async Task ValidateOverrideAsync_ShouldHandleDifferentValues_ForExitNote(
            double totalAvailableQuantity,
            double paramLimit, double paramThreshold, bool shouldCreateOverride)
    {
        // Arrange
        var note = new ExitNote { Id = 1, Product = new Product { Id = 1 }};
        var parameters = new Parameters { MaximumAmount = paramLimit, MinimumAmount = paramThreshold };
        var transaction = new Transactions { Id = 1, InventoryId = 1 };

        //Config mock
        _transactionsRepositoryMock
            .Setup(repo => repo.GetTransactionByNoteIdAndNoteType(note.Id, TransactionTypeEnum.Exit))
            .Returns(transaction);

        if (shouldCreateOverride)
            _overrideRepositoryMock
                .Setup(repo => repo.CreateOverrideAsync(It.IsAny<Override>()))
                .Returns(Task.CompletedTask)
                .Verifiable();

        // Act
        await _overrideService.ValidateOverrideAsync(note, totalAvailableQuantity, parameters);

        // Assert
        if (shouldCreateOverride)
            _overrideRepositoryMock.Verify(repo => repo.CreateOverrideAsync(It.IsAny<Override>()), Times.Once);
        else
            _overrideRepositoryMock.Verify(repo => repo.CreateOverrideAsync(It.IsAny<Override>()), Times.Never);
    }
}
