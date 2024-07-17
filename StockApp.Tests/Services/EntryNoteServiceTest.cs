using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Storage;
using Moq;
using Newtonsoft.Json.Linq;
using StockApp.API.Repositories.Interfaces;
using StockApp.API.Services;
using StockApp.API.Services.Interfaces;
using StockApp.Models.Dtos.BatchDto;
using StockApp.Models.Dtos.Note;
using StockApp.Models.Dtos.Pagination;
using StockApp.Models.Models;
using StockApp.Tests.Utils;

namespace StockApp.Tests.Services;

[TestFixture]
public class EntryNoteServiceTest
{
    private Mock<IEntryNoteRepository> _entryNoteRepositoryMock;
    private Mock<IMapper> _mapperMock;
    private Mock<IProductRepository> _productRepositoryMock;
    private Mock<IBatchService> _batchServiceMock;
    private Mock<IInventoryRepository> _inventoryRepositoryMock;
    private Mock<ITransactionsService> _transactionServiceMock;
    private Mock<IInventoryService> _inventoryServiceMock;
    private EntryNoteService _entryNoteService;
    
    
    [SetUp]
    public void SetUp()
    {
        //Mocks creation
        _entryNoteRepositoryMock = new Mock<IEntryNoteRepository>();
        _mapperMock = new Mock<IMapper>();
        _productRepositoryMock = new Mock<IProductRepository>();
        _batchServiceMock = new Mock<IBatchService>();
        _inventoryRepositoryMock = new Mock<IInventoryRepository>();
        _transactionServiceMock = new Mock<ITransactionsService>();
        _inventoryServiceMock = new Mock<IInventoryService>();

        // Services creation
        _entryNoteService = new EntryNoteService(
         _entryNoteRepositoryMock.Object,
         _mapperMock.Object,
         _productRepositoryMock.Object,
         _batchServiceMock.Object,
         _inventoryRepositoryMock.Object,
         _inventoryServiceMock.Object,
         _transactionServiceMock.Object
        );
    }

    [Test]
    public async Task CreateEntryNoteAsync_ReturnsOkObjectResult_WhenValidInput()
    {
        // Arrange
        var product = CreateArrange.CreateProduct();
        var inventory = CreateArrange.CreateInventory();
        var entryNote = CreateArrange.CreateEntryNote();
        var createEntryNoteDto = CreateArrange.CreateEntryNoteDto();
        var listEntryNoteDto = CreateArrange.CreateListEntryNoteDto(noteGenerationTime: entryNote.NoteGenerationTime, expiryDate: entryNote.ExpiryDate);
        var batch = CreateArrange.CreateBatch();
        var listInventoryBatchDto = new ListBatchDto
        {
            ProductId = batch.ProductId,
            Quantity = batch.Quantity,
            TotalValue = batch.TotalValue,
        };

        //Config mocks
        var dbContextTransactionMock = new Mock<IDbContextTransaction>();
        _entryNoteRepositoryMock.Setup(r => r.BeginTransactionAsync()).ReturnsAsync(dbContextTransactionMock.Object);

        _mapperMock.Setup(m => m.Map<EntryNote>(createEntryNoteDto)).Returns(entryNote);
        _mapperMock.Setup(m => m.Map<ListEntryNoteDto>(entryNote)).Returns(listEntryNoteDto);
        _entryNoteRepositoryMock.Setup(r => r.CreateEntryNoteAsync(entryNote)).Returns(Task.CompletedTask);
        _batchServiceMock.Setup(b => b.CreateBatchAsync(entryNote)).ReturnsAsync(batch);
        _mapperMock.Setup(m => m.Map<ListBatchDto>(batch)).Returns(listInventoryBatchDto);
        _productRepositoryMock.Setup(p => p.GetByIdAsync(product.Id)).ReturnsAsync(product);
        _inventoryRepositoryMock.Setup(i => i.GetInventoryByIdAsync(inventory.Id)).ReturnsAsync(inventory);

        // Act
        var result = await _entryNoteService.CreateEntryNoteAsync(createEntryNoteDto);

        // Assert
        var okResult = result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);
        Assert.That(okResult.Value, Is.Not.Null);

        var value = JObject.FromObject(okResult.Value);

        var entryNoteResult = value["EntryNote"]?.ToObject<ListEntryNoteDto>();
        Assert.That(entryNoteResult, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(entryNoteResult.NoteGenerationTime, Is.EqualTo(listEntryNoteDto.NoteGenerationTime));
            Assert.That(entryNoteResult.ExpiryDate, Is.EqualTo(listEntryNoteDto.ExpiryDate));
            Assert.That(entryNoteResult.ProductId, Is.EqualTo(listEntryNoteDto.ProductId));
            Assert.That(entryNoteResult.InventoryId, Is.EqualTo(listEntryNoteDto.InventoryId));
            Assert.That(entryNoteResult.TotalValue, Is.EqualTo(listEntryNoteDto.TotalValue));
        });

        var batchResult = value["Batch"]?.ToObject<ListBatchDto>();
        Assert.That(batchResult, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(batchResult.ProductId, Is.EqualTo(listInventoryBatchDto.ProductId));
            Assert.That(batchResult.Quantity, Is.EqualTo(listInventoryBatchDto.Quantity));
            Assert.That(batchResult.TotalValue, Is.EqualTo(listInventoryBatchDto.TotalValue));
        });
    }

    [Test]
    public async Task CreateEntryNoteAsync_ReturnsBadRequestObjectResult_WhenValidationFails()
    {
        // Arrange
        var createEntryNoteDto = CreateArrange.CreateEntryNoteDto();

        //Config mocks
        var dbContextTransactionMock = new Mock<IDbContextTransaction>();
        _entryNoteRepositoryMock.Setup(r => r.BeginTransactionAsync()).ReturnsAsync(dbContextTransactionMock.Object);
        _productRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Product?)null);

        // Act
        var result = await _entryNoteService.CreateEntryNoteAsync(createEntryNoteDto);

        // Assert
        Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
    }

    [Test]
    public async Task CreateEntryNoteAsync_ReturnsBadRequestObjectResult_WhenExceptionIsThrown()
    {
        // Arrange
        var createEntryNoteDto = CreateArrange.CreateEntryNoteDto();

        //Config mock
        var dbContextTransactionMock = new Mock<IDbContextTransaction>();
        _entryNoteRepositoryMock.Setup(r => r.BeginTransactionAsync()).ReturnsAsync(dbContextTransactionMock.Object);

        _mapperMock.Setup(m => m.Map<EntryNote>(createEntryNoteDto)).Throws(new Exception("Test exception."));

        // Act
        var result = await _entryNoteService.CreateEntryNoteAsync(createEntryNoteDto);

        // Assert
        Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
    }

    [Test]
    public async Task GetEntryNoteByIdAsync_ReturnsOkObjectResult_WhenValidId()
    {
        //Arrange
        var product = CreateArrange.CreateProduct();
        var inventory = CreateArrange.CreateInventory();
        var entryNote = CreateArrange.CreateEntryNote();
        var listEntryNoteDto = CreateArrange.CreateListEntryNoteDto(noteGenerationTime: entryNote.NoteGenerationTime, expiryDate: entryNote.ExpiryDate);

        //Config mock
        _mapperMock.Setup(m => m.Map<ListEntryNoteDto>(entryNote)).Returns(listEntryNoteDto);
        _entryNoteRepositoryMock.Setup(r => r.GetEntryNoteByIdAsync(entryNote.Id)).Returns(Task.FromResult(entryNote));

        //Act 
        var result = await _entryNoteService.GetEntryNoteByIdAsync(entryNote.Id);

        //Assert
        var okResult = result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);
        var entryNoteResult = okResult.Value as ListEntryNoteDto;
        Assert.That(entryNoteResult, Is.Not.Null);
        Assert.That(entryNoteResult, Is.EqualTo(listEntryNoteDto));
    }

    [Test]
    public async Task GetEntryNoteByIdAsync_ReturnsNotFoundResult_WhenEntryNoteNotFound()
    {
        //Arrange
        int entryNoteNotFound = 1;

        //Config mocks
        _entryNoteRepositoryMock.Setup(r => r.GetEntryNoteByIdAsync(entryNoteNotFound)).ReturnsAsync((EntryNote)null);

        //Act
        var result = await _entryNoteService.GetEntryNoteByIdAsync(entryNoteNotFound);

        //Assert
        Assert.That(result, Is.InstanceOf<NotFoundResult>());
    }

    [Test]
    public async Task GetAllEntryNotesAsync_ReturnsOkObjectResult_WithEntryNotes()
    {
        // Arrange
        var entryNotes = new List<EntryNote>
        {
            CreateArrange.CreateEntryNote(),
            CreateArrange.CreateEntryNote()
        };

        var entryNotesDto = entryNotes.Select(e => CreateArrange.CreateListEntryNoteDto(
            noteGenerationTime: e.NoteGenerationTime,
            expiryDate: e.ExpiryDate,
            price: e.Price,
            quantity: e.Quantity,
            productId: e.ProductId,
            inventoryId: e.InventoryId
            ));

        //Config mocks
        _entryNoteRepositoryMock.Setup(r => r.GetAllEntryNotesAsync(It.IsAny<int>(), It.IsAny<int>(), string.Empty, true)).ReturnsAsync((entryNotes.Count,entryNotes));
        _mapperMock.Setup(m => m.Map<IEnumerable<ListEntryNoteDto>>(entryNotes)).Returns(entryNotesDto);

        // Act
        var result = await _entryNoteService.GetAllEntryNotesAsync(skip: 0, take: 10, string.Empty, true);

        // Assert
        var okResult = result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);
        Assert.That(okResult.Value, Is.Not.Null);

        var value = okResult.Value as PagedResultDto<ListEntryNoteDto>;
        Assert.That(value, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(value.Records, Is.EqualTo(entryNotesDto));
            Assert.That(value.TotalRecords, Is.EqualTo(entryNotes.Count));
        });
    }
}
