using AutoMapper;
using Moq;
using StockApp.API.Repositories.Interfaces;
using StockApp.API.Services.Interfaces;
using StockApp.API.Services;
using Microsoft.AspNetCore.Mvc;
using StockApp.Models.Models;
using StockApp.Models.Dtos.Note;
using StockApp.Models.Dtos.BatchDto;
using StockApp.Models.Dtos.EntryNoteDto;
using StockApp.Models.Dtos.EntryNoteBatchDto;
using StockApp.Tests.Utils;

namespace StockApp.Tests.Services;

[TestFixture]
internal class ReverseExitTransactionTests
{
    private Mock<IExitNoteRepository> _exitNoteRepositoryMock;
    private Mock<IEntryNoteService> _entryNoteServiceMock;
    private Mock<IMapper> _mapper; 
    private ReverseExitTransaction _reverseExitTransaction;

    [SetUp]
    public void SetUp()
    {
        _exitNoteRepositoryMock = new Mock<IExitNoteRepository>();
        _entryNoteServiceMock = new Mock<IEntryNoteService>();
        _mapper = new Mock<IMapper>(); 
      

        _reverseExitTransaction = new ReverseExitTransaction(
            _exitNoteRepositoryMock.Object,
            _entryNoteServiceMock.Object,
            _mapper.Object
        );
    }

    [Test]
    public async Task ReverseExitNoteAsync_ExitNoteNotFound_ReturnsBadRequest()
    {
        // Arrange
        int exitNoteId = 1;

        //Config mock
        _exitNoteRepositoryMock.Setup(repo => repo.GetExitNoteByIdAsync(It.IsAny<int>())).ReturnsAsync((ExitNote?)null);

        // Act
        var result = await _reverseExitTransaction.ReverseExitNoteAsync(exitNoteId);

        // Assert
        Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
        var badRequestObjectResult = result as BadRequestObjectResult;
        Assert.That(badRequestObjectResult, Is.Not.Null);
        Assert.That(badRequestObjectResult.Value, Is.Not.Null);
        Assert.That(badRequestObjectResult.Value, Is.EqualTo("Exit note used by the transaction was not found."));
    }

    [Test]
    public async Task ReverseExitNoteAsync_CreatesEntryNotesSuccessfully()
    {
        //Arrange
        var batchObject1 = CreateArrange.CreateBatch();
        var batchObject2 = CreateArrange.CreateBatch(id: 2);
        var exitNoteBatches = new List<ExitNoteBatch>() { };

        var exitNote = CreateArrange.CreateExitNote(exitNoteBatches: exitNoteBatches, quantity: 20);
        var exitNoteBatch1 = CreateArrange.CreateExitNoteBatch(batch: batchObject1);
        var exitNoteBatch2 = CreateArrange.CreateExitNoteBatch(batch: batchObject2);
        exitNoteBatches.Add(exitNoteBatch1);
        exitNoteBatches.Add(exitNoteBatch2);

        var listEntryNoteDto1 = CreateArrange.CreateListEntryNoteDto(noteGenerationTime: DateTime.Now, expiryDate: null); 
        var listEntryNoteDto2 = CreateArrange.CreateListEntryNoteDto(id: 2, noteGenerationTime: DateTime.Now, expiryDate: null);

        var listBatchDto1 = new ListBatchDto { EntryNoteId = listEntryNoteDto1.Id, Quantity = listEntryNoteDto1.Quantity };
        var listBatchDto2 = new ListBatchDto { EntryNoteId = listEntryNoteDto2.Id, Quantity = listEntryNoteDto2.Quantity };

        var createEntryNoteDto1 = CreateArrange.CreateEntryNoteDto(); 
        var createEntryNoteDto2 = CreateArrange.CreateEntryNoteDto();

        var listEntryNoteBatchDto1 = new ListEntryNoteBatchDto { EntryNote = listEntryNoteDto1, Batch = listBatchDto1 };
        var listEntryNoteBatchDto2 = new ListEntryNoteBatchDto { EntryNote = listEntryNoteDto2, Batch = listBatchDto2 };

        //config mock
        _exitNoteRepositoryMock.Setup(s => s.GetExitNoteByIdAsync(exitNote.Id)).ReturnsAsync(exitNote);
        _entryNoteServiceMock.Setup(s => s.CreateEntryNoteAsync(createEntryNoteDto1)).ReturnsAsync(new OkObjectResult(listEntryNoteBatchDto1));
        _entryNoteServiceMock.Setup(s => s.CreateEntryNoteAsync(createEntryNoteDto2)).ReturnsAsync(new OkObjectResult(listEntryNoteBatchDto2));
        _mapper.Setup(m => m.Map<CreateEntryNoteDto>(exitNoteBatch1)).Returns(createEntryNoteDto1);
        _mapper.Setup(m => m.Map<CreateEntryNoteDto>(exitNoteBatch2)).Returns(createEntryNoteDto2);

        _mapper.Setup(m => m.Map<EntryNotesReversedDto>(It.IsAny<(List<ListEntryNoteDto> EntryNotes, List<ListBatchDto> Batches)>()))
          .Returns((ValueTuple<List<ListEntryNoteDto>, List<ListBatchDto>> src) =>
              new EntryNotesReversedDto
              {
                  EntryNotes = src.Item1,
                  Batches = src.Item2
              });

        //Act 
        var result = await _reverseExitTransaction.ReverseExitNoteAsync(exitNote.Id);

        Assert.That(result, Is.InstanceOf<OkObjectResult>());
        var okResultObject = result as OkObjectResult;
        Assert.That(okResultObject, Is.Not.Null);
        var resultValue = okResultObject.Value as EntryNotesReversedDto;
        Assert.That(resultValue, Is.Not.Null);

        // Assert EntryNotes
        Assert.That(resultValue.EntryNotes, Is.Not.Null);
        Assert.That(resultValue.EntryNotes, Has.Count.EqualTo(2));
        Assert.That(resultValue.EntryNotes.First, Is.EqualTo(listEntryNoteDto1));
        Assert.That(resultValue.EntryNotes.Last, Is.EqualTo(listEntryNoteDto2));

        // Assert Batches
        Assert.That(resultValue.Batches, Is.Not.Null);
        Assert.That(resultValue.Batches, Has.Count.EqualTo(2));
        Assert.That(resultValue.Batches.First, Is.EqualTo(listBatchDto1));
        Assert.That(resultValue.Batches.Last, Is.EqualTo(listBatchDto2));
    }

    [Test]
    public async Task ReverseExitNoteAsync_EntryNoteCreationFails_ReturnsError()
    {
        // Arrange
        var batchObject = CreateArrange.CreateBatch();
        var exitNoteBatch = CreateArrange.CreateExitNoteBatch(batch: batchObject);
        var exitNote = CreateArrange.CreateExitNote(exitNoteBatches: [exitNoteBatch]);
        var errorResult = new BadRequestObjectResult("Error creating entry note.");
        var createEntryNoteDto = CreateArrange.CreateEntryNoteDto();

        // Config mock
        _exitNoteRepositoryMock.Setup(repo => repo.GetExitNoteByIdAsync(It.IsAny<int>())).ReturnsAsync(exitNote);
        _mapper.Setup(m => m.Map<CreateEntryNoteDto>(exitNoteBatch)).Returns(createEntryNoteDto);
        _entryNoteServiceMock.Setup(service => service.CreateEntryNoteAsync(createEntryNoteDto)).ReturnsAsync(errorResult);

        // Act
        var result = await _reverseExitTransaction.ReverseExitNoteAsync(exitNote.Id);

        // Assert
        Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
        var badRequestObjectResult = result as BadRequestObjectResult;
        Assert.That(badRequestObjectResult, Is.Not.Null);
        Assert.That(badRequestObjectResult.Value, Is.EqualTo("Error creating entry note."));

        // Verify methods call
        _entryNoteServiceMock.Verify(service => service.CreateEntryNoteAsync(It.IsAny<CreateEntryNoteDto>()), Times.Once);
    }

    [Test]
    public async Task ReverseExitNoteAsync_EntryNoteWithZeroQuantity_SkipsCreation()
    {
        // Arrange
        int exitNoteId = 1;
        var batchObject = CreateArrange.CreateBatch(quantity: 0);
        var exitNoteBatch = CreateArrange.CreateExitNoteBatch(batch: batchObject, quantity: 0); 
        var exitNote = CreateArrange.CreateExitNote(exitNoteBatches: [exitNoteBatch], quantity: 0);
        var createEntryNoteDto = CreateArrange.CreateEntryNoteDto(quantity: 0);
        var entryNote = CreateArrange.CreateEntryNote(quantity: 0);
        var okResult = new OkObjectResult(entryNote);

        //Config mock
        _exitNoteRepositoryMock.Setup(repo => repo.GetExitNoteByIdAsync(It.IsAny<int>())).ReturnsAsync(exitNote);
        _mapper.Setup(m => m.Map<CreateEntryNoteDto>(exitNoteBatch)).Returns(createEntryNoteDto); 

        // Act
        var result = await _reverseExitTransaction.ReverseExitNoteAsync(exitNoteId);

        // Assert
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
        var okResultObject = result as OkObjectResult;
        Assert.That(okResultObject, Is.Not.Null);
        var entryNotesCreated = okResultObject.Value as List<EntryNote>;
        Assert.That(entryNotesCreated, Is.Null);

        // Verify methods call
        _entryNoteServiceMock.Verify(service => service.CreateEntryNoteAsync(It.IsAny<CreateEntryNoteDto>()), Times.Never);
    }

    [Test]
    public async Task CreateEntryNotesFromExitNoteAsync_AllBatchesHaveZeroQuantity_ReturnsEmptyResult()
    {
        // Arrange
        var batch1 = CreateArrange.CreateBatch(quantity: 0);
        var batch2 = CreateArrange.CreateBatch(quantity: 0);
        var exitNoteBatch1 = CreateArrange.CreateExitNoteBatch(batch1, quantity: 0);
        var exitNoteBatch2 = CreateArrange.CreateExitNoteBatch(batch2, quantity: 0);
        var exitNote = CreateArrange.CreateExitNote(exitNoteBatches: [exitNoteBatch1,exitNoteBatch2]);

        _mapper.Setup(m => m.Map<CreateEntryNoteDto>(exitNoteBatch1)).Returns(CreateArrange.CreateEntryNoteDto(quantity: 0));
        _mapper.Setup(m => m.Map<CreateEntryNoteDto>(exitNoteBatch2)).Returns(CreateArrange.CreateEntryNoteDto(quantity: 0));

        _mapper.Setup(m => m.Map<EntryNotesReversedDto>(It.IsAny<(List<ListEntryNoteDto> EntryNotes, List<ListBatchDto> Batches)>()))
            .Returns((ValueTuple<List<ListEntryNoteDto>, List<ListBatchDto>> src) =>
                new EntryNotesReversedDto
                {
                    EntryNotes = src.Item1,
                    Batches = src.Item2
                });

        // Act
        var result = await _reverseExitTransaction.CreateEntryNotesFromExitNoteAsync(exitNote);

        // Assert
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
        var okResult = result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);
        var entryNotesReversedDto = okResult.Value as EntryNotesReversedDto;
        Assert.That(entryNotesReversedDto, Is.Not.Null);
        Assert.That(entryNotesReversedDto.EntryNotes, Is.Empty);
        Assert.That(entryNotesReversedDto.Batches, Is.Empty);

        _entryNoteServiceMock.Verify(service => service.CreateEntryNoteAsync(It.IsAny<CreateEntryNoteDto>()), Times.Never);
    }

    [Test]
    public async Task CreateEntryNotesFromExitNoteAsync_ExitNoteHasNoBatches_ReturnsEmptyResult()
    {
        // Arrange
        var exitNote = CreateArrange.CreateExitNote(exitNoteBatches: []);

        //Config mock
        _mapper.Setup(m => m.Map<EntryNotesReversedDto>(It.IsAny<(List<ListEntryNoteDto> EntryNotes, List<ListBatchDto> Batches)>()))
            .Returns((ValueTuple<List<ListEntryNoteDto>, List<ListBatchDto>> src) =>
                new EntryNotesReversedDto
                {
                    EntryNotes = src.Item1,
                    Batches = src.Item2
                });

        // Act
        var result = await _reverseExitTransaction.CreateEntryNotesFromExitNoteAsync(exitNote);

        // Assert
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
        var okResult = result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);
        var entryNotesReversedDto = okResult.Value as EntryNotesReversedDto;
        Assert.That(entryNotesReversedDto, Is.Not.Null);
        Assert.That(entryNotesReversedDto.EntryNotes, Is.Empty);
        Assert.That(entryNotesReversedDto.Batches, Is.Empty);
    }

    [Test]
    public async Task CreateEntryNotesFromExitNoteAsync_EntryNotesCreatedSuccessfully_ReturnsCreatedEntryNotes()
    {
        // Arrange
        var batch1 = CreateArrange.CreateBatch();
        var batch2 = CreateArrange.CreateBatch(quantity: 20);
        var exitNoteBatch1 = CreateArrange.CreateExitNoteBatch(batch1);
        var exitNoteBatch2 = CreateArrange.CreateExitNoteBatch(batch2, quantity: 20);
        var exitNote = CreateArrange.CreateExitNote(exitNoteBatches: [exitNoteBatch1, exitNoteBatch2]);

        var createEntryNoteDto1 = CreateArrange.CreateEntryNoteDto();
        var createEntryNoteDto2 = CreateArrange.CreateEntryNoteDto(quantity: 20);

        var listEntryNoteDto1 = CreateArrange.CreateListEntryNoteDto(noteGenerationTime: DateTime.Now, expiryDate: null);
        var listEntryNoteDto2 = CreateArrange.CreateListEntryNoteDto(id: 2, noteGenerationTime: DateTime.Now, expiryDate: null);

        var listBatchDto1 = new ListBatchDto { EntryNoteId = listEntryNoteDto1.Id, Quantity = listEntryNoteDto1.Quantity };
        var listBatchDto2 = new ListBatchDto { EntryNoteId = listEntryNoteDto2.Id, Quantity = listEntryNoteDto2.Quantity };

        var listEntryNoteBatchDto1 = new ListEntryNoteBatchDto { EntryNote = listEntryNoteDto1, Batch = listBatchDto1 };
        var listEntryNoteBatchDto2 = new ListEntryNoteBatchDto { EntryNote = listEntryNoteDto2, Batch = listBatchDto2 };

        _mapper.Setup(m => m.Map<CreateEntryNoteDto>(exitNoteBatch1)).Returns(createEntryNoteDto1);
        _mapper.Setup(m => m.Map<CreateEntryNoteDto>(exitNoteBatch2)).Returns(createEntryNoteDto2);

        _entryNoteServiceMock.Setup(s => s.CreateEntryNoteAsync(createEntryNoteDto1)).ReturnsAsync(new OkObjectResult(listEntryNoteBatchDto1));
        _entryNoteServiceMock.Setup(s => s.CreateEntryNoteAsync(createEntryNoteDto2)).ReturnsAsync(new OkObjectResult(listEntryNoteBatchDto2));

        _mapper.Setup(m => m.Map<EntryNotesReversedDto>(It.IsAny<(List<ListEntryNoteDto> EntryNotes, List<ListBatchDto> Batches)>()))
            .Returns((ValueTuple<List<ListEntryNoteDto>, List<ListBatchDto>> src) =>
                new EntryNotesReversedDto
                {
                    EntryNotes = src.Item1,
                    Batches = src.Item2
                });

        // Act
        var result = await _reverseExitTransaction.CreateEntryNotesFromExitNoteAsync(exitNote);

        // Assert
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
        var okResult = result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);
        var entryNotesReversedDto = okResult.Value as EntryNotesReversedDto;
        Assert.That(entryNotesReversedDto, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(entryNotesReversedDto.EntryNotes, Has.Count.EqualTo(2));
            Assert.That(entryNotesReversedDto.Batches, Has.Count.EqualTo(2));
            Assert.That(entryNotesReversedDto.EntryNotes.First(), Is.EqualTo(listEntryNoteDto1));
            Assert.That(entryNotesReversedDto.EntryNotes.Last(), Is.EqualTo(listEntryNoteDto2));
            Assert.That(entryNotesReversedDto.Batches.First(), Is.EqualTo(listBatchDto1));
            Assert.That(entryNotesReversedDto.Batches.Last(), Is.EqualTo(listBatchDto2));
        });
    }

    [Test]
    public async Task CreateEntryNotesFromExitNoteAsync_EntryNoteCreationFails_ReturnsError()
    {
        // Arrange
        var batch = CreateArrange.CreateBatch();
        var exitNoteBatch = CreateArrange.CreateExitNoteBatch(batch);
        var exitNote = CreateArrange.CreateExitNote(exitNoteBatches: [exitNoteBatch]);
        var createEntryNoteDto = CreateArrange.CreateEntryNoteDto();
        var errorResult = new BadRequestObjectResult("Error creating entry note");

        //Config mock
        _mapper.Setup(m => m.Map<CreateEntryNoteDto>(exitNoteBatch)).Returns(createEntryNoteDto);
        _entryNoteServiceMock.Setup(service => service.CreateEntryNoteAsync(createEntryNoteDto)).ReturnsAsync(errorResult);

        // Act
        var result = await _reverseExitTransaction.CreateEntryNotesFromExitNoteAsync(exitNote);

        // Assert
        Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
        var badRequestObjectResult = result as BadRequestObjectResult;
        Assert.That(badRequestObjectResult, Is.Not.Null);
        Assert.That(badRequestObjectResult.Value, Is.EqualTo("Error creating entry note"));

        // Verify methods call
        _entryNoteServiceMock.Verify(service => service.CreateEntryNoteAsync(It.IsAny<CreateEntryNoteDto>()), Times.Once);
    }
}
