using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Moq;
using StockApp.API.Repositories.Interfaces;
using StockApp.API.Services;
using StockApp.Models.Dtos.Pagination;
using StockApp.Models.Dtos.Transactions;
using StockApp.Models.Enum;
using StockApp.Models.Models;

namespace StockApp.Tests.Services;

public class TransactionsServiceTests
{
    private Mock<ITransactionsRepository> _transactionRepositoryMock;

    private Mock<IMapper> _mapperMock;

    private Mock<IEntryNoteRepository> _entryNoteRepositoryMock;

    private Mock<IExitNoteRepository> _exitNoteRepository;

    private Mock<IOverrideRepository> _overrideRepository;

    private TransactionsService _transactionsService;
    
    [SetUp]
    public void SetUp()
    {
        _transactionRepositoryMock = new Mock<ITransactionsRepository>();
        _mapperMock = new Mock<IMapper>();
        _entryNoteRepositoryMock = new Mock<IEntryNoteRepository>();
        _exitNoteRepository = new Mock<IExitNoteRepository>();
        _overrideRepository = new Mock<IOverrideRepository>();
        _transactionsService = new TransactionsService(_transactionRepositoryMock.Object, _mapperMock.Object, _entryNoteRepositoryMock.Object, _exitNoteRepository.Object, _overrideRepository.Object);
    }

    [Test]
    public async Task CreateEntryNoteTransaction()
    {
        // Arrange
        var entryNote = new EntryNote()
        {
            Id = 1,
            InventoryId = 1,
            Quantity = 10,
            Price = 10,
            ProductId = 1,
            ExpiryDate = DateTime.Now
        };
        
        string expectedDescription = $"Entry Note: {entryNote.Id} added {entryNote.Quantity} to Inventory: {entryNote.InventoryId}.";
        
        _transactionRepositoryMock.Setup(r => r.CreateTransaction(It.IsAny<Transactions>()));

        // Act
        await _transactionsService.CreateTransaction(entryNote, TransactionTypeEnum.Entry);

        // Assert
        _transactionRepositoryMock.Verify(r => r.CreateTransaction(It.Is<Transactions>(t =>
            t.TransactionType == TransactionTypeEnum.Entry &&
            t.InventoryId == entryNote.InventoryId &&
            t.NoteId == entryNote.Id &&
            t.Description == expectedDescription &&
            t.Quantity == entryNote.Quantity
        )), Times.Once);
    }
    
    [Test]
    public async Task CreateExitNoteTransaction()
    {
        // Arrange
        var entryNote = new EntryNote()
        {
            Id = 1,
            InventoryId = 1,
            Quantity = 10,
            Price = 10,
            ProductId = 1,
            ExpiryDate = DateTime.Now
        };
        
        string expectedDescription = $"Exit Note: {entryNote.Id} removed {entryNote.Quantity} to Inventory: {entryNote.InventoryId}.";
        
        _transactionRepositoryMock.Setup(r => r.CreateTransaction(It.IsAny<Transactions>()));

        // Act
        await _transactionsService.CreateTransaction(entryNote, TransactionTypeEnum.Exit);

        // Assert
        _transactionRepositoryMock.Verify(r => r.CreateTransaction(It.Is<Transactions>(t =>
            t.TransactionType == TransactionTypeEnum.Exit &&
            t.InventoryId == entryNote.InventoryId &&
            t.NoteId == entryNote.Id &&
            t.Description == expectedDescription &&
            t.Quantity == entryNote.Quantity
        )), Times.Once);
    }
    
    [Test]
    public async Task GetAllTransaction_ReturnsOkResult_WithListOfTransactions()
    {
        // Arrange
        var transactions = new List<Transactions>
        {
            new() { TransactionType = TransactionTypeEnum.Entry, NoteId = 1, InventoryId = 1, Quantity = 10 },
            new() { TransactionType = TransactionTypeEnum.Exit, NoteId = 2, InventoryId = 1, Quantity = 5 }
        };

        var transactionDtos = new List<ListTransactionsDto>
        {
            new() { TransactionType = TransactionTypeEnum.Entry, NoteId = 1, InventoryId = 1, Quantity = 10 },
            new() { TransactionType = TransactionTypeEnum.Exit, NoteId = 2, InventoryId = 1, Quantity = 5 }
        };

        int skip = 0;
        int take = 10;

        _transactionRepositoryMock.Setup(repo => repo.GetAllTransactions(skip,take, String.Empty, true)).ReturnsAsync((transactions.Count,transactions));
        _mapperMock.Setup(mapper => mapper.Map<Transactions, ListTransactionsDto>(It.IsAny<Transactions>()))
            .Returns((Transactions src) => new ListTransactionsDto
            {
                TransactionType = src.TransactionType,
                NoteId = src.NoteId,
                InventoryId = src.InventoryId,
                Quantity = src.Quantity
            });

        // Act
        var result = await _transactionsService.GetAllTransaction(skip, take, string.Empty, true) as OkObjectResult;

        // Assert
        Assert.That(result, Is.Not.Null);
        var pagedList = result.Value as PagedResultDto<ListTransactionsDto>;
        Assert.That(pagedList, Is.Not.Null);
        Assert.That(pagedList.TotalRecords, Is.EqualTo(transactionDtos.Count));
        var transactionsList = pagedList.Records as List<ListTransactionsDto>;
        Assert.That(transactionsList, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(transactionsList[0].Quantity, Is.EqualTo(transactionDtos[0].Quantity));
            Assert.That(transactionsList[0].TransactionType, Is.EqualTo(transactionDtos[0].TransactionType));
            Assert.That(transactionsList[1].Quantity, Is.EqualTo(transactionDtos[1].Quantity));
            Assert.That(transactionsList[1].TransactionType, Is.EqualTo(transactionDtos[1].TransactionType));
        });
    }
    
    [Test]
        public async Task GetAllTransactionsByType_ValidTransactionType_ReturnsOkResult_WithListOfTransactions()
        {
            // Arrange
            var transactionType = TransactionTypeEnum.Entry;
            var transactions = new List<Transactions>
            {
                new Transactions { TransactionType = TransactionTypeEnum.Entry, NoteId = 1, InventoryId = 1, Quantity = 10 },
                new Transactions { TransactionType = TransactionTypeEnum.Entry, NoteId = 2, InventoryId = 1, Quantity = 20 }
            };

            var transactionDtos = new List<ListTransactionsDto>
            {
                new ListTransactionsDto { TransactionType = TransactionTypeEnum.Entry, NoteId = 1, InventoryId = 1, Quantity = 10 },
                new ListTransactionsDto { TransactionType = TransactionTypeEnum.Entry, NoteId = 2, InventoryId = 1, Quantity = 20 }
            };

            _transactionRepositoryMock.Setup(repo => repo.GetAllTransactionsByType(transactionType)).ReturnsAsync(transactions);
            _mapperMock.Setup(mapper => mapper.Map<Transactions, ListTransactionsDto>(It.IsAny<Transactions>()))
                       .Returns((Transactions src) => new ListTransactionsDto
                       {
                           TransactionType = src.TransactionType,
                           NoteId = src.NoteId,
                           InventoryId = src.InventoryId,
                           Quantity = src.Quantity
                       });

            // Act
            var result = await _transactionsService.GetAllTransactionsByType(transactionType) as OkObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.That(result.StatusCode, Is.EqualTo(200));
            Assert.IsInstanceOf<List<ListTransactionsDto>>(result.Value);
            var returnList = result.Value as List<ListTransactionsDto>;
            Assert.That(returnList.Count, Is.EqualTo(transactionDtos.Count));
        }

        [Test]
        public async Task GetAllTransactionsByType_InvalidTransactionType_ReturnsBadRequest()
        {
            // Arrange
            var invalidTransactionType = (TransactionTypeEnum)999;

            // Act
            var result = await _transactionsService.GetAllTransactionsByType(invalidTransactionType) as BadRequestObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.That(result.StatusCode, Is.EqualTo(400));
            Assert.That(result.Value, Is.EqualTo("Invalid Transaction Type"));
        }
        
        [Test]
        public async Task GetAllTransactionsByType_NoTransactions_ReturnsOkResult_WithEmptyList()
        {
            // Arrange
            var transactionType = TransactionTypeEnum.Entry;
            var emptyTransactionsList = new List<Transactions>();

            _transactionRepositoryMock.Setup(repo => repo.GetAllTransactionsByType(transactionType)).ReturnsAsync(emptyTransactionsList);

            // Act
            var result = await _transactionsService.GetAllTransactionsByType(transactionType) as OkObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.That(result.StatusCode, Is.EqualTo(200));
            Assert.IsInstanceOf<List<ListTransactionsDto>>(result.Value);
            var returnList = result.Value as List<ListTransactionsDto>;
            Assert.IsEmpty(returnList);
        }
        
        [Test]
        public async Task GetTransactionById_TransactionExists_ReturnsOkResult_WithTransactionDto()
        {
            // Arrange
            int transactionId = 1;
            var transaction = new Transactions
            {
                TransactionType = TransactionTypeEnum.Entry,
                NoteId = 1,
                InventoryId = 1,
                Quantity = 10
            };

            var transactionDto = new ListTransactionsDto
            {
                TransactionType = TransactionTypeEnum.Entry,
                NoteId = 1,
                InventoryId = 1,
                Quantity = 10
            };

            _transactionRepositoryMock.Setup(repo => repo.GetTransactionById(transactionId)).Returns(transaction);
            _mapperMock.Setup(mapper => mapper.Map<Transactions, ListTransactionsDto>(transaction)).Returns(transactionDto);

            // Act
            var result = await _transactionsService.GetTransactionById(transactionId) as OkObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.That(result.StatusCode, Is.EqualTo(200));
            Assert.IsInstanceOf<ListTransactionsDto>(result.Value);
            Assert.That(result.Value, Is.EqualTo(transactionDto));
        }

        [Test]
        public async Task GetTransactionByNoteIdAndNoteType_ValidTransaction_ReturnsOkResult_WithTransactionDto()
        {
            // Arrange
            int noteId = 1;
            var transactionType = TransactionTypeEnum.Entry;
            var transaction = new Transactions
            {
                TransactionType = transactionType,
                NoteId = noteId,
                InventoryId = 1,
                Quantity = 10
            };

            var transactionDto = new ListTransactionsDto
            {
                TransactionType = transactionType,
                NoteId = noteId,
                InventoryId = 1,
                Quantity = 10
            };

            _transactionRepositoryMock.Setup(repo => repo.GetTransactionByNoteIdAndNoteType(noteId, transactionType)).Returns(transaction);
            _mapperMock.Setup(mapper => mapper.Map<Transactions, ListTransactionsDto>(transaction)).Returns(transactionDto);

            // Act
            var result = await _transactionsService.GetTransactionByNoteIdAndNoteType(noteId, transactionType) as OkObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.That(result.StatusCode, Is.EqualTo(200));
            Assert.IsInstanceOf<ListTransactionsDto>(result.Value);
            Assert.That(result.Value, Is.EqualTo(transactionDto));
        }

        [Test]
        public async Task GetTransactionByNoteIdAndNoteType_TransactionDoesNotExist_ReturnsNotFoundResult()
        {
            // Arrange
            int noteId = 1;
            var transactionType = TransactionTypeEnum.Entry;

            _transactionRepositoryMock.Setup(repo => repo.GetTransactionByNoteIdAndNoteType(noteId, transactionType)).Returns((Transactions)null);

            // Act
            var result = await _transactionsService.GetTransactionByNoteIdAndNoteType(noteId, transactionType) as NotFoundObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.That(result.StatusCode, Is.EqualTo(404));
            Assert.That(result.Value, Is.EqualTo($"There isn't a note with Id: {noteId} and TransactionType: {transactionType.ToString()}"));
        }

        [Test]
        public async Task GetTransactionByNoteIdAndNoteType_InvalidTransactionType_ReturnsBadRequestResult()
        {
            // Arrange
            int noteId = 1;
            var invalidTransactionType = (TransactionTypeEnum)999;

            // Act
            var result = await _transactionsService.GetTransactionByNoteIdAndNoteType(noteId, invalidTransactionType) as BadRequestObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.That(result.StatusCode, Is.EqualTo(400));
            Assert.That(result.Value, Is.EqualTo("Invalid Transaction Type"));
        }



}