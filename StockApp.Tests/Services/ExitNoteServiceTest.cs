using AutoMapper;
using Moq;
using StockApp.API.Repositories.Interfaces;
using StockApp.API.Services.Interfaces;
using StockApp.API.Services;
using StockApp.Models.Dtos.ExitNote;
using StockApp.Models.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Storage;
using StockApp.Tests.Utils;
using StockApp.Models.Dtos.Pagination;

namespace StockApp.Tests.Services;

[TestFixture]
public class ExitNoteServiceTest
{
    private Mock<IExitNoteRepository> _exitNoteRepositoryMock;
    private Mock<IProductRepository> _productRepositoryMock;
    private Mock<IBatchService> _batchServiceMock;
    private Mock<IInventoryService> _inventoryServiceMock;
    private Mock<IExitNoteBatchService> _exitNoteBatchServiceMock;
    private Mock<IMapper> _mapperMock;
    private Mock<ITransactionsService> _transactionService;
    private ExitNoteService _exitNoteService;

    [SetUp]
    public void SetUp()
    {
        _exitNoteRepositoryMock = new Mock<IExitNoteRepository>();
        _productRepositoryMock = new Mock<IProductRepository>();
        _batchServiceMock = new Mock<IBatchService>();
        _inventoryServiceMock = new Mock<IInventoryService>();
        _exitNoteBatchServiceMock = new Mock<IExitNoteBatchService>();
        _mapperMock = new Mock<IMapper>();
        _transactionService = new Mock<ITransactionsService>();
        
        _exitNoteService = new ExitNoteService(
            _exitNoteRepositoryMock.Object,
            _productRepositoryMock.Object,
            _mapperMock.Object,
            _exitNoteBatchServiceMock.Object,
            _batchServiceMock.Object,
            _inventoryServiceMock.Object,
            _transactionService.Object
        );
       
    }

    private static List<Batch> CreateBatches(int count)
    {
        var batches = new List<Batch>();
        for (int i = 0; i < count; i++)
        {
            batches.Add(CreateArrange.CreateBatch());
        }
        return batches;
    }

    [Test]
    public async Task CreateExitNoteAsync_ReturnsOkObjectResult_WhenSuccessful()
    {
        // Arrange
        var product = CreateArrange.CreateProduct();
        var inventory = CreateArrange.CreateInventory();
        var batches = CreateBatches(2);
        var usedBatches = new List<UsedBatch>()
        {
            new()
            {
                Batch = batches[0], 
                QuantityUsed = batches[0].Quantity
            }, 
            new ()
            {
                Batch = batches[1],
                QuantityUsed = batches[1].Quantity
            }
        };
        var exitNote = CreateArrange.CreateExitNote();
        var createExitNoteDto = CreateArrange.CreateExitNoteDto();
        var listExitNoteDto = CreateArrange.CreateListExitNoteDto();
        var exitNoteBatch = new ExitNoteBatch { ExitNoteId = exitNote.Id }; 
        //Config mock
        var dbContextTransactionMock = new Mock<IDbContextTransaction>();
        _exitNoteRepositoryMock.Setup(r => r.BeginTransactionAsync()).ReturnsAsync(dbContextTransactionMock.Object);

        _mapperMock.Setup(m => m.Map<ExitNote>(createExitNoteDto)).Returns(exitNote);
        _mapperMock.Setup(m => m.Map<ListExitNoteDto>(exitNote)).Returns(listExitNoteDto);

        _exitNoteRepositoryMock.Setup(r => r.CreateExitNoteAsync(exitNote)).Returns(Task.CompletedTask);
        _inventoryServiceMock.Setup(s => s.ValidateInventoryParametersAsync(exitNote)).ReturnsAsync(new OkResult());
        _inventoryServiceMock.Setup(s => s.ValidateAvailableBatchesAsync(exitNote.InventoryId, exitNote.ProductId, exitNote.Quantity)).ReturnsAsync((batches, new OkResult()));
        _inventoryServiceMock.Setup(s => s.GetInventoryByIdAsync(It.IsAny<int>())).ReturnsAsync(new OkObjectResult(inventory));
        _batchServiceMock.Setup(s => s.DeductQuantityFromBatchesAsync(batches, exitNote.Quantity)).ReturnsAsync(usedBatches);
        _exitNoteBatchServiceMock.Setup(s => s.CreateExitNoteBatchAsync(exitNote, It.IsAny<List<UsedBatch>>())).ReturnsAsync(new OkObjectResult(exitNoteBatch));
        _productRepositoryMock.Setup(s => s.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(product);

        // Act
        var result = await _exitNoteService.CreateExitNoteAsync(createExitNoteDto);
        var resltBad = result as BadRequestObjectResult;

        // Assert
        var okResult = result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);
        Assert.That(okResult.Value, Is.Not.Null);

        var resultValue = okResult.Value as ListExitNoteDto;
        Assert.That(resultValue, Is.Not.Null);
        Assert.That(resultValue, Is.EqualTo(listExitNoteDto));
    }

    [Test]
    public async Task CreateExitNoteAsync_ReturnsBadRequest_WhenProductNotFound()
    {
        //Arrange
        var exitNote = CreateArrange.CreateExitNote(productId: 0);
        var createExitNoteDto = CreateArrange.CreateExitNoteDto(productId: 0);

        //Config mock
        var dbContextTransactionMock = new Mock<IDbContextTransaction>();
        _exitNoteRepositoryMock.Setup(r => r.BeginTransactionAsync()).ReturnsAsync(dbContextTransactionMock.Object);

        _mapperMock.Setup(m => m.Map<ExitNote>(createExitNoteDto)).Returns(exitNote);
        _productRepositoryMock.Setup(s => s.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Product)null);

        // Act
        var result = await _exitNoteService.CreateExitNoteAsync(createExitNoteDto);

        // Assert
        var badRequestResult = result as BadRequestObjectResult;
        Assert.That(badRequestResult, Is.Not.Null);
        Assert.That(badRequestResult.Value, Is.EqualTo("Product not found."));
    }

    [Test]
    public async Task CreateExitNoteAsync_ReturnsBadRequest_WhenInventoryNotFound()
    {
        //Arrange
        var product = CreateArrange.CreateProduct();
        var createExitNoteDto = CreateArrange.CreateExitNoteDto(inventoryId: 0);
        var exitNote = CreateArrange.CreateExitNote(inventoryId: 0);

        //Config mock
        var dbContextTransactionMock = new Mock<IDbContextTransaction>();
        _exitNoteRepositoryMock.Setup(r => r.BeginTransactionAsync()).ReturnsAsync(dbContextTransactionMock.Object);

        _mapperMock.Setup(m => m.Map<ExitNote>(createExitNoteDto)).Returns(exitNote);
        _productRepositoryMock.Setup(s => s.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(product);
        _inventoryServiceMock.Setup(s => s.GetInventoryByIdAsync(It.IsAny<int>())).ReturnsAsync(new NotFoundResult());

        // Act
        var result = await _exitNoteService.CreateExitNoteAsync(createExitNoteDto);

        // Assert
        var badRequestResult = result as BadRequestObjectResult;
        Assert.That(badRequestResult, Is.Not.Null);
        Assert.That(badRequestResult.Value, Is.EqualTo("Inventory not found."));
    }

    [Test]
    public async Task CreateExitNoteAsync_ReturnsBadRequest_WhenInsufficientBatches()
    {
        // Arrange
        var product = CreateArrange.CreateProduct();
        var inventory = CreateArrange.CreateInventory();
        var createExitNoteDto = CreateArrange.CreateExitNoteDto(quantity: 11);
        var exitNote = CreateArrange.CreateExitNote(quantity: 11);
        var batches = CreateBatches(2);

        //Config mock
        var dbContextTransactionMock = new Mock<IDbContextTransaction>();
        _exitNoteRepositoryMock.Setup(r => r.BeginTransactionAsync()).ReturnsAsync(dbContextTransactionMock.Object);

        _mapperMock.Setup(m => m.Map<ExitNote>(createExitNoteDto)).Returns(exitNote);
        _productRepositoryMock.Setup(s => s.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(product);
        _inventoryServiceMock.Setup(s => s.GetInventoryByIdAsync(It.IsAny<int>())).ReturnsAsync(new OkObjectResult(inventory));
        _inventoryServiceMock.Setup(s => s.ValidateAvailableBatchesAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<double>())).ReturnsAsync((batches, new BadRequestObjectResult("Insufficient batches")));

        // Act
        var result = await _exitNoteService.CreateExitNoteAsync(createExitNoteDto);

        // Assert
        var badRequestResult = result as BadRequestObjectResult;
        Assert.That(badRequestResult, Is.Not.Null);
        Assert.That(badRequestResult.Value, Is.EqualTo("Insufficient batches"));
    }

    [Test]
    public async Task CreateExitNoteAsync_ReturnsInternalServerError_WhenExitNoteBatchCreationFails()
    {
        // Arrange
        var product = CreateArrange.CreateProduct();
        var inventory = CreateArrange.CreateInventory();
        var createExitNoteDto = CreateArrange.CreateExitNoteDto();
        var exitNote = CreateArrange.CreateExitNote();
        var batches = CreateBatches(0);
        var usedBatch = new List<UsedBatch>(); 

        //Config mock
        var dbContextTransactionMock = new Mock<IDbContextTransaction>();
        _exitNoteRepositoryMock.Setup(r => r.BeginTransactionAsync()).ReturnsAsync(dbContextTransactionMock.Object);

        _mapperMock.Setup(m => m.Map<ExitNote>(createExitNoteDto)).Returns(exitNote);
        _productRepositoryMock.Setup(s => s.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(product);
        _inventoryServiceMock.Setup(s => s.GetInventoryByIdAsync(It.IsAny<int>())).ReturnsAsync(new OkObjectResult(inventory));
        _inventoryServiceMock.Setup(s => s.ValidateAvailableBatchesAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<double>())).ReturnsAsync((batches, new OkResult()));
        _batchServiceMock.Setup(s => s.DeductQuantityFromBatchesAsync(It.IsAny<IEnumerable<Batch>>(), It.IsAny<double>())).ReturnsAsync(usedBatch);
        _exitNoteBatchServiceMock.Setup(s => s.CreateExitNoteBatchAsync(It.IsAny<ExitNote>(), It.IsAny<List<UsedBatch>>())).ReturnsAsync(new StatusCodeResult(500));

        // Act
        var result = await _exitNoteService.CreateExitNoteAsync(createExitNoteDto);

        // Assert
        var statusCodeResult = result as StatusCodeResult;
        Assert.That(statusCodeResult, Is.Not.Null);
        Assert.That(statusCodeResult.StatusCode, Is.EqualTo(500));
    }

    [Test]
    public void CreateExitNoteAsync_RollsBackTransaction_WhenExceptionThrown()
    {
        // Arrange
        var createExitNoteDto = CreateArrange.CreateExitNoteDto();

        var dbContextTransactionMock = new Mock<IDbContextTransaction>();
        _exitNoteRepositoryMock.Setup(r => r.BeginTransactionAsync()).ReturnsAsync(dbContextTransactionMock.Object);

        _exitNoteRepositoryMock.Setup(r => r.BeginTransactionAsync()).ReturnsAsync(dbContextTransactionMock.Object);
        _mapperMock.Setup(m => m.Map<ExitNote>(It.IsAny<CreateExitNoteDto>())).Throws(new Exception("Test exception"));

        // Act
        async Task<IActionResult> Act() => await _exitNoteService.CreateExitNoteAsync(createExitNoteDto);

        // Assert
        var exception = Assert.ThrowsAsync<Exception>(Act);
        Assert.That(exception.Message, Is.EqualTo("Test exception"));
        dbContextTransactionMock.Verify(t => t.RollbackAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task GetAllExitNotesAsync_ReturnsOkObjectResult_WithExitNotes()
    {
        // Arrange
        var expectedExitNotes = new List<ExitNote>()
        {
            CreateArrange.CreateExitNote(),
            CreateArrange.CreateExitNote()
        };

        var expectedExitNotesListDto = new List<ListExitNoteDto>()
        {
            CreateArrange.CreateListExitNoteDto(),
            CreateArrange.CreateListExitNoteDto(),
        };

        //Config mock
        _exitNoteRepositoryMock.Setup(repo => repo.GetAllExitNotesAsync(It.IsAny<int>(), It.IsAny<int>(), string.Empty, true)).ReturnsAsync((expectedExitNotes.Count, expectedExitNotes));
        _mapperMock.Setup(m => m.Map<IEnumerable<ListExitNoteDto>>(expectedExitNotes)).Returns(expectedExitNotesListDto);

        // Act
        var result = await _exitNoteService.GetAllExitNotesAsync(skip: 0, take: 10, String.Empty, true);
        var okResult = result as OkObjectResult;

        // Assert
        Assert.That(okResult, Is.Not.Null);
        var pagedResult = okResult.Value as PagedResultDto<ListExitNoteDto>; 
        Assert.That(pagedResult, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(pagedResult.Records, Is.EqualTo(expectedExitNotesListDto));
            Assert.That(pagedResult.TotalRecords, Is.EqualTo(expectedExitNotes.Count));
        });
    }

    [Test]
    public async Task GetExitNoteByIdAsync_ReturnsOkObjectResult_WhenValidId()
    {
        // Arrange
        var expectedExitNote = CreateArrange.CreateExitNote();
        var expectedListExitNote = CreateArrange.CreateListExitNoteDto();

        //Config mock
        _mapperMock.Setup(m => m.Map<ListExitNoteDto>(expectedExitNote)).Returns(expectedListExitNote);
        _exitNoteRepositoryMock.Setup(repo => repo.GetExitNoteByIdAsync(It.IsAny<int>())).ReturnsAsync(expectedExitNote);

        // Act
        var result = await _exitNoteService.GetExitNoteByIdAsync(1);

        // Assert
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
        var okResult = result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null); 
        Assert.That(okResult.Value, Is.EqualTo(expectedListExitNote));
    }

    [Test]
    public async Task GetExitNoteByIdAsync_ReturnsNotFoundResult_WhenInvalidId()
    {
        //Arrange
        int exitNoteNotFoundId = 1;

        //Config mocks
        _exitNoteRepositoryMock.Setup(r => r.GetExitNoteByIdAsync(exitNoteNotFoundId)).ReturnsAsync((ExitNote?)null);

        //Act
        var result = await _exitNoteService.GetExitNoteByIdAsync(exitNoteNotFoundId);

        //Assert
        Assert.That(result, Is.InstanceOf<NotFoundResult>());
    }
}
