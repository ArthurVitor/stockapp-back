using AutoMapper;
using Moq;
using StockApp.API.Repositories.Interfaces;
using StockApp.API.Services.Interfaces;
using StockApp.API.Services;
using Microsoft.AspNetCore.Mvc;
using StockApp.Models.Dtos.Transactions;
using StockApp.Models.Models;
using StockApp.Models.Enum;
using StockApp.Models.Dtos.AdminRoute;
using StockApp.Models.Dtos.ExitNote;
using StockApp.Models.Dtos.Note;
using StockApp.Models.Dtos.BatchDto;
using StockApp.Models.Dtos.Pagination;
using StockApp.Tests.Utils;

namespace StockApp.Tests.Services;


[TestFixture]
internal class AdminRouteServiceTests
{
    private Mock<IEntryNoteService> _entryNoteServiceMock;
    private Mock<IExitNoteService> _exitNoteServiceMock;
    private Mock<IMapper> _mapperMock;
    private Mock<IReverseEntryTransacion> _reverseEntryTransacionMock;
    private Mock<IReverseExitTransaction> _reverseExitTransactionMock;
    private Mock<ITransactionsService> _transactionsServiceMock;
    private Mock<ITransactionsRepository> _transactionsRepositoryMock;
    private Mock<IAdminRouteRepository> _adminRouteRepositoryMock;
    private AdminRouteService _adminRouteService;

    [SetUp]
    public void SetUp()
    {
        _entryNoteServiceMock = new Mock<IEntryNoteService>();
        _exitNoteServiceMock = new Mock<IExitNoteService>();
        _mapperMock = new Mock<IMapper>();
        _reverseEntryTransacionMock = new Mock<IReverseEntryTransacion>();
        _reverseExitTransactionMock = new Mock<IReverseExitTransaction>();
        _transactionsServiceMock = new Mock<ITransactionsService>();
        _transactionsRepositoryMock = new Mock<ITransactionsRepository>();
        _adminRouteRepositoryMock = new Mock<IAdminRouteRepository>();

        _adminRouteService = new AdminRouteService(
            _exitNoteServiceMock.Object,
            _reverseExitTransactionMock.Object,
            _reverseEntryTransacionMock.Object,
            _entryNoteServiceMock.Object,
            _transactionsServiceMock.Object,
            _transactionsRepositoryMock.Object,
            _mapperMock.Object,
            _adminRouteRepositoryMock.Object
        );
    }

    [Test]
    public async Task ReverseTransactionAsync_TransactionNotFound_ReturnsNotFound()
    {
        // Arrange
        var notFoundId = 1;
        var reverseTransactionDto = new ReverseTransactionDto { OverrideId = notFoundId };

        //Config mock
        _transactionsServiceMock.Setup(x => x.GetTransactionByOverrideId(It.IsAny<int>()))
            .ReturnsAsync(new NotFoundObjectResult($"There's no transaction with ID {notFoundId}"));

        // Act
        var result = await _adminRouteService.ReverseTransactionAsync(reverseTransactionDto);

        // Assert
        Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());
        var notFoundResult = result as NotFoundObjectResult;
        Assert.That(notFoundResult, Is.Not.Null);
        Assert.That(notFoundResult.Value, Is.EqualTo($"There's no transaction with ID {notFoundId}"));
    }

    [Test]
    public async Task ReverseTransactionAsync_TransactionAlreadyReversed_ReturnsBadRequest()
    {
        // Arrange
        var reverseTransactionDto = new ReverseTransactionDto { OverrideId = 1 };
        var transaction = new Transactions { Reversed = true };

        //Config mock
        _transactionsServiceMock.Setup(x => x.GetTransactionByOverrideId(It.IsAny<int>()))
            .ReturnsAsync(transaction);

        // Act
        var result = await _adminRouteService.ReverseTransactionAsync(reverseTransactionDto);

        // Assert
        Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
        var badRequestResult = result as BadRequestObjectResult;
        Assert.That(badRequestResult, Is.Not.Null);
        Assert.That(badRequestResult.Value, Is.EqualTo("This transaction has already been reversed."));
    }

    [Test]
    public async Task ReverseTransactionAsync_EntryTransaction_ReversesEntryNote()
    {
        // Arrange
        var reverseTransactionDto = new ReverseTransactionDto { OverrideId = 1 };
        var transaction = new Transactions { Reversed = false, TransactionType = TransactionTypeEnum.Entry, NoteId = 1 };

        //Config mock
        _transactionsServiceMock.Setup(x => x.GetTransactionByOverrideId(It.IsAny<int>()))
            .ReturnsAsync(transaction);

        // Act
        var result = await _adminRouteService.ReverseTransactionAsync(reverseTransactionDto);

        // Assert
        _transactionsRepositoryMock.Verify(x => x.UpdateReversedTransactionAsync(transaction), Times.Once);
        _reverseEntryTransacionMock.Verify(x => x.ReverseEntryNoteAsync(transaction.NoteId), Times.Once);
    }

    [Test]
    public async Task ReverseTransactionAsync_ExitTransaction_ReversesExitNote()
    {
        // Arrange
        var reverseTransactionDto = new ReverseTransactionDto { OverrideId = 1 };
        var transaction = new Transactions { Reversed = false, TransactionType = TransactionTypeEnum.Exit, NoteId = 1 };

        //Config mock
        _transactionsServiceMock.Setup(x => x.GetTransactionByOverrideId(It.IsAny<int>()))
            .ReturnsAsync(transaction);

        // Act
        var result = await _adminRouteService.ReverseTransactionAsync(reverseTransactionDto);

        // Assert
        _transactionsRepositoryMock.Verify(x => x.UpdateReversedTransactionAsync(transaction), Times.Once);
        _reverseExitTransactionMock.Verify(x => x.ReverseExitNoteAsync(transaction.NoteId), Times.Once);
    }

    [Test]
    public async Task ReallocateBatch_SameInventoryIds_ReturnsBadRequest()
    {
        //Arrange
        var reallocateDto = CreateArrange.CreateReallocateProductDto(expiryDate: DateTime.Now, senderInventoryId: 1, recipientInventoryId: 1);

        //Act
        var result = await _adminRouteService.ReallocateBatch(reallocateDto);

        //Assert
        var badRequestResult = result as BadRequestObjectResult;
        Assert.That(badRequestResult, Is.Not.Null);
        Assert.That(badRequestResult.Value, Is.EqualTo("It is not possible to reallocate a batch to its originating inventory."));
    }

    [Test]
    public async Task ReallocateBatch_ExitNoteServiceFails_ReturnsFailureResult()
    {
        //Arrange
        var reallocateDto = CreateArrange.CreateReallocateProductDto(expiryDate: DateTime.Now);

        //Config mock
        _mapperMock.Setup(m => m.Map<CreateExitNoteDto>(It.IsAny<CreateReallocateProductDto>()))
                  .Returns(new CreateExitNoteDto());
        _exitNoteServiceMock.Setup(s => s.CreateExitNoteAsync(It.IsAny<CreateExitNoteDto>()))
                          .ReturnsAsync(new BadRequestObjectResult("Exit note creation failed"));


        //Act
        var result = await _adminRouteService.ReallocateBatch(reallocateDto);

        //Assert
        var badRequestResult = result as BadRequestObjectResult;
        Assert.That(badRequestResult, Is.Not.Null);
        Assert.That(badRequestResult.Value, Is.EqualTo("Exit note creation failed"));
    }

    [Test]
    public async Task ReallocateBatch_EntryNoteServiceFails_ReturnsFailureResult()
    {
        // Arrange
        var reallocateDto = CreateArrange.CreateReallocateProductDto(expiryDate: DateTime.Now);

        //Config mock
        _mapperMock.Setup(m => m.Map<CreateExitNoteDto>(It.IsAny<CreateReallocateProductDto>()))
                   .Returns(new CreateExitNoteDto());
        _exitNoteServiceMock.Setup(s => s.CreateExitNoteAsync(It.IsAny<CreateExitNoteDto>()))
                            .ReturnsAsync(new OkObjectResult(null));
        _mapperMock.Setup(m => m.Map<CreateEntryNoteDto>(It.IsAny<CreateReallocateProductDto>()))
                   .Returns(new CreateEntryNoteDto());
        _entryNoteServiceMock.Setup(s => s.CreateEntryNoteAsync(It.IsAny<CreateEntryNoteDto>()))
                             .ReturnsAsync(new BadRequestObjectResult("Entry note creation failed"));

        // Act
        var result = await _adminRouteService.ReallocateBatch(reallocateDto);

        // Assert
        var badRequestResult = result as BadRequestObjectResult;
        Assert.That(badRequestResult, Is.Not.Null);
        Assert.That(badRequestResult.Value, Is.EqualTo("Entry note creation failed"));
    }

    [Test]
    public async Task ReallocateBatch_SuccessfulReallocation_ReturnsOkResult()
    {
        // Arrange
        var reallocateDto = CreateArrange.CreateReallocateProductDto(expiryDate: DateTime.Now);

        _mapperMock.Setup(m => m.Map<CreateExitNoteDto>(It.IsAny<CreateReallocateProductDto>()))
                   .Returns(new CreateExitNoteDto());
        _exitNoteServiceMock.Setup(s => s.CreateExitNoteAsync(It.IsAny<CreateExitNoteDto>()))
                            .ReturnsAsync(new OkObjectResult(null));
        _mapperMock.Setup(m => m.Map<CreateEntryNoteDto>(It.IsAny<CreateReallocateProductDto>()))
                   .Returns(new CreateEntryNoteDto());
        _entryNoteServiceMock.Setup(s => s.CreateEntryNoteAsync(It.IsAny<CreateEntryNoteDto>()))
                             .ReturnsAsync(new OkObjectResult(null));

        // Act
        var result = await _adminRouteService.ReallocateBatch(reallocateDto);

        // Assert
        Assert.That(result as OkResult, Is.Not.Null);
    }

    [Test]
    public async Task GetAllExpiredBatches_ReturnsPagedResult()
    {
        // Arrange
        var skip = 0;
        var take = 10;
        var orderBy = "ExpiryDate";
        var isAscending = true;

        var expiredBatches = new List<Batch> {
            CreateArrange.CreateBatch(expiryDate: DateTime.Now.AddDays(-1)),
            CreateArrange.CreateBatch(id:2, expiryDate:DateTime.Now.AddDays(-2))
        };

        var listBatchDtos = new List<ListBatchDto> {
            new() {Id = 1, ExpiryDate = DateTime.Now.AddDays(-1)},
            new() {Id = 2,  ExpiryDate = DateTime.Now.AddDays(-2)}
        };

        //Config mock
        _adminRouteRepositoryMock.Setup(r => r.GetAllExpiredBatches(skip, take, orderBy, isAscending))
            .ReturnsAsync((expiredBatches.Count, expiredBatches));

        _mapperMock.Setup(m => m.Map<IEnumerable<ListBatchDto>>(expiredBatches))
            .Returns(listBatchDtos);

        // Act
        var result = await _adminRouteService.GetAllExpiredBatches(skip, take, orderBy, isAscending);

        // Assert
        var okResult = result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);
        var pagedResult = okResult.Value as PagedResultDto<ListBatchDto>;
        Assert.That(pagedResult, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(expiredBatches, Has.Count.EqualTo(pagedResult.TotalRecords));
            Assert.That(listBatchDtos, Is.EqualTo(pagedResult.Records));
        });
    }

    [Test]
    public async Task DeleteAllPerishedBatches_ReturnsNoContent()
    {
        // Arrange
        var expiredBatches = new List<Batch>
        {
            CreateArrange.CreateBatch(expiryDate:DateTime.Now.AddDays(-1)),
            CreateArrange.CreateBatch(id:2, expiryDate:DateTime.Now.AddDays(-2)),
        };

        //Config mock
        _adminRouteRepositoryMock.Setup(r => r.GetAllExpiredBatches())
            .ReturnsAsync(expiredBatches);

        _adminRouteRepositoryMock.Setup(r => r.BulkDeletePerishedBatches(expiredBatches))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _adminRouteService.DeleteAllPerishedBatches();

        // Assert
        Assert.That(result as NoContentResult, Is.Not.Null);
    }
}
