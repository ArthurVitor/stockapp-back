using AutoMapper;
using Moq;
using StockApp.API.Repositories.Interfaces;
using StockApp.API.Services.Interfaces;
using StockApp.API.Services;
using Microsoft.AspNetCore.Mvc;
using StockApp.Models.Models;
using StockApp.Models.Dtos.ExitNote;
using StockApp.Tests.Utils;

namespace StockApp.Tests.Services;

[TestFixture]
internal class ReverseEntryTransactionTests
{
    private Mock<IEntryNoteRepository> _entryNoteRepositoryMock;
    private Mock<IExitNoteService> _exitNoteServiceMock;
    private Mock<IMapper> _mapperMock;
    private ReverseEntryTransaction _reverseEntryTransaction;

    [SetUp]
    public void SetUp()
    {
        _entryNoteRepositoryMock = new Mock<IEntryNoteRepository>();
        _exitNoteServiceMock = new Mock<IExitNoteService>();
        _mapperMock = new Mock<IMapper>();

        _reverseEntryTransaction = new ReverseEntryTransaction(
            _entryNoteRepositoryMock.Object,
            _exitNoteServiceMock.Object,
            _mapperMock.Object
        );
    }

    [Test]
    public async Task ReverseEntryNoteAsync_EntryNoteNotFound_ReturnsBadRequest()
    {
        // Arrange
        int entryNoteId = 1;

        //Config mock
        _entryNoteRepositoryMock
            .Setup(repo => repo.GetEntryNoteByIdAsync(It.IsAny<int>()))
            .ReturnsAsync((EntryNote?)null);

        // Act
        var result = await _reverseEntryTransaction.ReverseEntryNoteAsync(entryNoteId);

        // Assert
        Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
        var badRequestResult = result as BadRequestObjectResult;
        Assert.That(badRequestResult, Is.Not.Null);
        Assert.That(badRequestResult.Value, Is.EqualTo("Entry note used by the transaction was not found."));
    }

    [Test]
    public async Task ReverseEntryNoteAsync_CreatesExitNoteSuccessfully_ReturnsOkObjectResult()
    {
        // Arrange
        var entryNote = CreateArrange.CreateEntryNote();
        var listExitNoteDto = CreateArrange.CreateListExitNoteDto(); 
        var createExitNoteDto = CreateArrange.CreateExitNoteDto();
        var okObjectResult = new OkObjectResult(listExitNoteDto);

        _entryNoteRepositoryMock
            .Setup(repo => repo.GetEntryNoteByIdAsync(It.IsAny<int>()))
            .ReturnsAsync(entryNote);

        _mapperMock
            .Setup(mapper => mapper.Map<CreateExitNoteDto>(It.IsAny<EntryNote>()))
            .Returns(createExitNoteDto);

        _exitNoteServiceMock
            .Setup(service => service.CreateExitNoteAsync(It.IsAny<CreateExitNoteDto>()))
            .ReturnsAsync(okObjectResult);

        // Act
        var result = await _reverseEntryTransaction.ReverseEntryNoteAsync(entryNote.Id);

        // Assert
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
        var okObjectObject = result as OkObjectResult;
        Assert.That(okObjectObject, Is.Not.Null);
    }

    [Test]
    public async Task ReverseEntryNoteAsync_FailsToCreateExitNote_ReturnsBadRequest()
    {
        // Arrange
        var entryNote = CreateArrange.CreateEntryNote(); 
        var createExitNoteDto = CreateArrange.CreateExitNoteDto();
        var badRequestResult = new BadRequestResult();

        _entryNoteRepositoryMock
            .Setup(repo => repo.GetEntryNoteByIdAsync(It.IsAny<int>()))
            .ReturnsAsync(entryNote);

        _mapperMock
            .Setup(mapper => mapper.Map<CreateExitNoteDto>(It.IsAny<EntryNote>()))
            .Returns(createExitNoteDto);

        _exitNoteServiceMock
            .Setup(service => service.CreateExitNoteAsync(It.IsAny<CreateExitNoteDto>()))
            .ReturnsAsync(badRequestResult);

        // Act
        var result = await _reverseEntryTransaction.ReverseEntryNoteAsync(entryNote.Id);

        // Assert
        Assert.That(result, Is.InstanceOf<BadRequestResult>());
    }

    [Test]
    public async Task CreateExitNoteFromEntryNoteAsync_CreatesExitNoteSuccessfully_ReturnsOkObjectResult()
    {
        // Arrange
        var entryNote = CreateArrange.CreateEntryNote(); 
        var createExitNoteDto = CreateArrange.CreateExitNoteDto();
        var okObjectResult = new OkObjectResult(CreateArrange.CreateListExitNoteDto());

        _mapperMock
            .Setup(mapper => mapper.Map<CreateExitNoteDto>(It.IsAny<EntryNote>()))
            .Returns(createExitNoteDto);

        _exitNoteServiceMock
            .Setup(service => service.CreateExitNoteAsync(It.IsAny<CreateExitNoteDto>()))
            .ReturnsAsync(okObjectResult);

        // Act
        var result = await _reverseEntryTransaction.CreateExitNoteFromEntryNoteAsync(entryNote);

        // Assert
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
        var okObjectObject = result as OkObjectResult;
        Assert.That(okObjectObject, Is.Not.Null);
    }

    [Test]
    public async Task CreateExitNoteFromEntryNoteAsync_FailsToCreateExitNote_ReturnsBadRequest()
    {
        // Arrange
        var entryNote = CreateArrange.CreateEntryNote();
        var createExitNoteDto = CreateArrange.CreateExitNoteDto();
        var badRequestResult = new BadRequestResult();

        _mapperMock
            .Setup(mapper => mapper.Map<CreateExitNoteDto>(It.IsAny<EntryNote>()))
            .Returns(createExitNoteDto);

        _exitNoteServiceMock
            .Setup(service => service.CreateExitNoteAsync(It.IsAny<CreateExitNoteDto>()))
            .ReturnsAsync(badRequestResult);

        // Act
        var result = await _reverseEntryTransaction.CreateExitNoteFromEntryNoteAsync(entryNote);

        // Assert
        Assert.That(result, Is.InstanceOf<BadRequestResult>());
    }
}
