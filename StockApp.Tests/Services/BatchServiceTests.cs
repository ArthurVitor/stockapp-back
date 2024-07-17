using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Moq;
using StockApp.API.Repositories.Interfaces;
using StockApp.API.Services;
using StockApp.Models.Dtos.BatchDto;
using StockApp.Models.Dtos.Pagination;
using StockApp.Models.Models;
using StockApp.Tests.Utils;

namespace StockApp.Tests.Services;

[TestFixture]
public class BatchServiceTests
{
    private Mock<IMapper> _mapperMock;
    private Mock<IBatchRepository> _batchRepositoryMock;
    private BatchService _batchService;

    [SetUp]
    public void SetUp()
    {
        _mapperMock = new Mock<IMapper>();
        _batchRepositoryMock = new Mock<IBatchRepository>();
        _batchService = new BatchService(_mapperMock.Object, _batchRepositoryMock.Object);
        var batch = CreateArrange.CreateBatch();
    }

    [Test]
    public async Task CreateBatchAsync_ShouldCreateBatchCorrectly()
    {
        // Arrange
        var batch = CreateArrange.CreateBatch();
        var entryNote = new EntryNote { Id = 1, Quantity = batch.Quantity, Price = batch.Price };
        var createBatchDto = new CreateBatchDto { Quantity = batch.Quantity, Price = batch.Price };

        //Config mock
        _mapperMock.Setup(m => m.Map<CreateBatchDto>(entryNote)).Returns(createBatchDto);
        _mapperMock.Setup(m => m.Map<Batch>(createBatchDto)).Returns(batch);
        _batchRepositoryMock.Setup(r => r.CreateBatchAsync(batch)).Returns(Task.CompletedTask);

        // Act
        var result = await _batchService.CreateBatchAsync(entryNote);

        // Assert
        Assert.That(result, Is.EqualTo(batch));
        _batchRepositoryMock.Verify(r => r.CreateBatchAsync(batch), Times.Once);
    }

    [Test]
    public async Task GetAvaliableBatchByIdAsync_ShouldReturnOkObjectResult_WithBatch()
    {
        // Arrange
        var batch = CreateArrange.CreateBatch();
        var batchDto = new ListBatchDto();

        //Config mock
        _batchRepositoryMock.Setup(r => r.GetAvaliableBatchByIdAsync(It.IsAny<int>())).ReturnsAsync(batch);
        _mapperMock.Setup(m => m.Map<ListBatchDto>(batch)).Returns(batchDto);

        // Act
        var result = await _batchService.GetAvaliableBatchByIdAsync(1);

        // Assert
        var okResult = result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);
        Assert.That(okResult.Value, Is.EqualTo(batchDto));
    }

    [Test]
    public async Task GetAvaliableBatchByIdAsync_ShouldReturnNotFound_WhenBatchDoesNotExist()
    {
        // Arrange
        _batchRepositoryMock.Setup(r => r.GetAvaliableBatchByIdAsync(It.IsAny<int>())).ReturnsAsync((Batch?)null);

        // Act
        var result = await _batchService.GetAvaliableBatchByIdAsync(1);

        // Assert
        var notFoundResult = result as NotFoundResult;
        Assert.That(notFoundResult, Is.Not.Null);
    }

    [Test]
    public async Task GetExpiredBatchByIdAsync_BatchExists_ReturnsOkObjectResult()
    {
        // Arrange
        var batchId = 1;
        var batch = CreateArrange.CreateBatch();
        var batchDto = new ListBatchDto();

        //Config mock
        _batchRepositoryMock.Setup(r => r.GetExpiredBatchByIdAsync(batchId))
            .ReturnsAsync(batch);

        _mapperMock.Setup(m => m.Map<ListBatchDto>(batch))
            .Returns(batchDto);

        // Act
        var result = await _batchService.GetExpiredBatchByIdAsync(batchId);

        // Assert
        var okResult = result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);
        var batchValue = okResult.Value as ListBatchDto;
        Assert.That(batchValue, Is.Not.Null);
        Assert.That(batchValue, Is.EqualTo(batchDto));
    }

    [Test]
    public async Task GetExpiredBatchByIdAsync_BatchDoesNotExist_ReturnsNotFoundResult()
    {
        // Arrange
        var batchId = 1;
        _batchRepositoryMock.Setup(r => r.GetExpiredBatchByIdAsync(batchId))
            .ReturnsAsync((Batch)null);

        // Act
        var result = await _batchService.GetExpiredBatchByIdAsync(batchId);

        // Assert
        Assert.That(result as NotFoundResult, Is.Not.Null);
    }

    [Test]
    public async Task GetAllAvaliableBatchesAsync_ShouldReturnOkObjectResult_WithAvaliableBatches()
    {
        // Arrange
        var batches = new List<Batch> { CreateArrange.CreateBatch(), CreateArrange.CreateBatch()};
        var batchesDto = new List<ListBatchDto> { new(), new() };
        var pagedResultDto = new PagedResultDto<ListBatchDto>(batchesDto.Count, batchesDto); 

        //Config mock
        _batchRepositoryMock.Setup(r => r.GetAllAvaliableBatchesAsync(It.IsAny<int>(), It.IsAny<int>(), string.Empty, true)).ReturnsAsync((batches.Count, batches));
        _mapperMock.Setup(m => m.Map<IEnumerable<ListBatchDto>>(batches)).Returns(batchesDto);

        // Act
        var result = await _batchService.GetAllAvaliableBatchesAsync(0, 10, string.Empty, true);

        // Assert
        var okResult = result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);
        var pagedResult = okResult.Value as PagedResultDto<ListBatchDto>;
        Assert.That(pagedResult, Is.Not.Null);
        Assert.That(pagedResult.Records, Is.EqualTo(batchesDto));
        Assert.That(pagedResult.TotalRecords, Is.EqualTo(batchesDto.Count));
    }

    [Test]
    public async Task DeleteBatchByIdAsync_BatchExists_ReturnsNoContentResult()
    {
        // Arrange
        var batchId = 1;
        var batch = CreateArrange.CreateBatch(); 

        //Config mock
        _batchRepositoryMock.Setup(r => r.GetBatchByIdAsync(batchId))
            .ReturnsAsync(batch);

        _batchRepositoryMock.Setup(r => r.DeleteBatchAsync(batch))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _batchService.DeleteBatchByIdAsync(batchId);

        // Assert
        Assert.That(result as NoContentResult, Is.Not.Null);

        //Verify mock calls
        _batchRepositoryMock.Verify(r => r.GetBatchByIdAsync(batchId), Times.Once);
        _batchRepositoryMock.Verify(r => r.DeleteBatchAsync(batch), Times.Once);
    }

    [Test]
    public async Task DeleteBatchByIdAsync_BatchDoesNotExist_ReturnsNotFoundResult()
    {
        // Arrange
        var batchId = 1;

        _batchRepositoryMock.Setup(r => r.GetBatchByIdAsync(batchId))
            .ReturnsAsync((Batch)null);

        // Act
        var result = await _batchService.DeleteBatchByIdAsync(batchId);
        Console.WriteLine(result);

        // Assert
        Assert.That(result as NotFoundResult, Is.Not.Null);
        _batchRepositoryMock.Verify(r => r.GetBatchByIdAsync(batchId), Times.Once);
        _batchRepositoryMock.Verify(r => r.DeleteBatchAsync(It.IsAny<Batch>()), Times.Never);
    }

    [Test]
    public async Task DeductQuantityFromBatchesAsync_ShouldDeductQuantitiesCorrectly()
    {
        // Arrange
        var batch1 = CreateArrange.CreateBatch(quantity: 100, expiryDate: DateTime.Now.AddDays(1), price: 10);
        var batch2 = CreateArrange.CreateBatch(quantity: 200, expiryDate: DateTime.Now.AddDays(2), price: 10);
        var batches = new List<Batch> { batch1, batch2 };
        var quantityToDeduct = 150;

        _batchRepositoryMock.Setup(r => r.UpdateBatchAsync(It.IsAny<Batch>())).Returns(Task.CompletedTask);

        // Act
        var result = await _batchService.DeductQuantityFromBatchesAsync(batches, quantityToDeduct);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Has.Count.EqualTo(2));
            Assert.That(result.Any(ub => ub.Batch.Id == batch1.Id && ub.QuantityUsed == 100), Is.True);
            Assert.That(result.Any(ub => ub.Batch.Id == batch2.Id && ub.QuantityUsed == 50), Is.True);
            Assert.That(batch1.Quantity, Is.EqualTo(0));
            Assert.That(batch2.Quantity, Is.EqualTo(150));
            Assert.That(batch1.TotalValue, Is.EqualTo(0));
            Assert.That(batch2.TotalValue, Is.EqualTo(1500));
        });

        _batchRepositoryMock.Verify(r => r.DeleteBatchAsync(batch1), Times.Once);
        _batchRepositoryMock.Verify(r => r.UpdateBatchAsync(batch2), Times.Once);
    }

    [TestCase(20, 50, 30, 60)]
    [TestCase(100, 150, 50, 100)]
    [TestCase(50, 40, 0, 0)]
    [TestCase(1, 30, 29, 50.75, 1.75)]
    public async Task DeductQuantityFromBatchesAsync_CalculatesTotalValueCorrectly_ForQuantityToDeductGreaterThan0(double quantityToDeduct,
        double batchQuantity, double expectedQuantity, double expectedTotalValue, double price = 2.0)
    {
        // Arrange
        var entryNote1 = new EntryNote();
        var batches = new List<Batch>
        {
            CreateArrange.CreateBatch(
            expiryDate: DateTime.Today.AddDays(10),
            quantity: batchQuantity,
            price: price),
        };

        // Act
        var usedBatches = await _batchService.DeductQuantityFromBatchesAsync(batches, quantityToDeduct);

        // Assert
        Assert.That(usedBatches, Has.Count.EqualTo(1));
        var updatedBatch = usedBatches.First();
        Assert.Multiple(() =>
        {
            Assert.That(updatedBatch.Batch.Quantity, Is.EqualTo(expectedQuantity));
            Assert.That(updatedBatch.Batch.TotalValue, Is.EqualTo(expectedTotalValue));
        });

        // Verify repository calls
        if (expectedQuantity > 0)
            _batchRepositoryMock.Verify(r => r.UpdateBatchAsync(It.Is<Batch>(b => b.Id == 1 && b.Quantity == expectedQuantity && b.TotalValue == expectedTotalValue)), Times.Once);
        else
            _batchRepositoryMock.Verify(r => r.DeleteBatchAsync(It.Is<Batch>(b => b.Id == 1 && b.Quantity == expectedQuantity && b.TotalValue == expectedTotalValue)), Times.Once);
    }

    [Test]
    [TestCaseSource(nameof(TestDataDeductQuantityFromBatchesAsync))]
    public async Task DeductQuantityFromBatchesAsync_UsesBatchWithClosestExpiryDateFirst(Batch[] batches, double quantityToDeduct, Batch[] expectedUsedBatches)
    {
        // Act
        var usedBatches = await _batchService.DeductQuantityFromBatchesAsync([.. batches], quantityToDeduct);

        // Assert
        Assert.That(usedBatches, Has.Count.EqualTo(expectedUsedBatches.Length));
        for (int i = 0; i < expectedUsedBatches.Length; i++)
            Assert.That(usedBatches[i].Batch.Id, Is.EqualTo(expectedUsedBatches[i].Id));

        // Verify repository calls
        foreach (var batch in usedBatches)
        {
            if (batch.Batch.Quantity == 0)
                _batchRepositoryMock.Verify(r => r.DeleteBatchAsync(It.Is<Batch>(b => b.Id == batch.Batch.Id && b.Quantity == 0)), Times.Once);
            else
                _batchRepositoryMock.Verify(r => r.UpdateBatchAsync(It.Is<Batch>(b => b.Id == batch.Batch.Id)), Times.Once);
        }
    }

    private static IEnumerable<TestCaseData> TestDataDeductQuantityFromBatchesAsync()
    {
        yield return new TestCaseData(
            new Batch[]
            {
            CreateArrange.CreateBatch(id: 1, expiryDate: DateTime.Today.AddDays(10), quantity: 50),
            CreateArrange.CreateBatch(id: 2, expiryDate: DateTime.Today.AddDays(5), quantity: 30),
            CreateArrange.CreateBatch(id: 3, expiryDate: DateTime.Today.AddDays(20), quantity: 20)
            },
            40,
            new Batch[]
            {
            CreateArrange.CreateBatch(id: 2, expiryDate: DateTime.Today.AddDays(5), quantity: 0),
            CreateArrange.CreateBatch(id: 1, expiryDate: DateTime.Today.AddDays(10), quantity: 40)
            }
        ).SetName("FirstBatchDeletedSecondBatchUpdated");

        yield return new TestCaseData(
            new Batch[]
            {
                CreateArrange.CreateBatch(id: 1, expiryDate: DateTime.Today.AddDays(10), quantity: 50),
                CreateArrange.CreateBatch(id: 2, expiryDate: DateTime.Today.AddDays(5), quantity: 30),
                CreateArrange.CreateBatch(id: 3, expiryDate: DateTime.Today.AddDays(20), quantity: 20)
            },
            25,
            new Batch[]
            {
                CreateArrange.CreateBatch(id: 2, expiryDate: DateTime.Today.AddDays(5), quantity: 5)
            }
        ).SetName("DeductQuantityLessThanClosestBatch");


        yield return new TestCaseData(
            new Batch[]
            {
                CreateArrange.CreateBatch(id: 1, expiryDate: DateTime.Today.AddDays(10), quantity: 50),
                CreateArrange.CreateBatch(id: 2, expiryDate: DateTime.Today.AddDays(5), quantity: 30),
                CreateArrange.CreateBatch(id: 3, expiryDate: DateTime.Today.AddDays(20), quantity: 20)
            },
            80,
            new Batch[]
            {
                CreateArrange.CreateBatch(id: 2, expiryDate: DateTime.Today.AddDays(5), quantity: 0),
                CreateArrange.CreateBatch(id: 1, expiryDate: DateTime.Today.AddDays(10), quantity: 0)
            }
        ).SetName("DeductQuantityEqualsSumOfTwoBatches");

        yield return new TestCaseData(
            new Batch[]
            {
                CreateArrange.CreateBatch(id: 1, expiryDate: DateTime.Today.AddDays(10), quantity: 50)
            },
            20,
            new Batch[]
            {
                CreateArrange.CreateBatch(id: 1, expiryDate: DateTime.Today.AddDays(10), quantity: 30)
            }
        ).SetName("SingleBatch");

        yield return new TestCaseData(
            new Batch[]
            {
                CreateArrange.CreateBatch(id: 1, expiryDate: DateTime.Today.AddDays(10), quantity: 50),
                CreateArrange.CreateBatch(id: 2, expiryDate: DateTime.Today.AddDays(10), quantity: 30),
                CreateArrange.CreateBatch(id: 3, expiryDate: DateTime.Today.AddDays(10), quantity: 20)
            },
            70,
            new Batch[]
            {
                CreateArrange.CreateBatch(id: 1, expiryDate: DateTime.Today.AddDays(10), quantity: 0),
                CreateArrange.CreateBatch(id: 2, expiryDate: DateTime.Today.AddDays(10), quantity: 10)
            }
        ).SetName("SameExpiryDateBatches");
    }
}