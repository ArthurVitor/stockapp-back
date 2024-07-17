using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Moq;
using StockApp.API.Migrations;
using StockApp.API.Repositories.Interfaces;
using StockApp.API.Services;
using StockApp.API.Services.Interfaces;
using StockApp.Models.Dtos.BatchDto;
using StockApp.Models.Dtos.InventoryDto;
using StockApp.Models.Dtos.Pagination;
using StockApp.Models.Models;
using StockApp.Tests.Utils;

namespace StockApp.Tests.Services
{
    [TestFixture]
    public class InventoryServiceTests
    {
        private Mock<IInventoryRepository> _inventoryRepositoryMock;
        private Mock<IParametersRepository> _parametersRepositoryMock;
        private Mock<IBatchRepository> _batchRepositoryMock;
        private Mock<IMapper> _mapperMock;
        private InventoryService _inventoryService;
        private Mock<IOverrideService> _overrideService;

        [SetUp]
        public void SetUp()
        {
            _inventoryRepositoryMock = new Mock<IInventoryRepository>();
            _parametersRepositoryMock = new Mock<IParametersRepository>();
            _mapperMock = new Mock<IMapper>();
            _batchRepositoryMock = new Mock<IBatchRepository>();
            _parametersRepositoryMock = new Mock<IParametersRepository>();
            _overrideService = new Mock<IOverrideService>();

            _inventoryService = new InventoryService(_inventoryRepositoryMock.Object, _mapperMock.Object, _batchRepositoryMock.Object, _parametersRepositoryMock.Object, _overrideService.Object);
        }

        [Test]
        public async Task CreateInventoryAsync_ReturnsOkObjectResult_WhenValidInput()
        {
            // Arrange
            var createInventoryDto = new CreateInventoryDto();
            var inventory = CreateArrange.CreateInventory();
            var listInventoryDto = CreateArrange.CreateListInventoryDto(batches: []);

            //Config mock
            _mapperMock.Setup(m => m.Map<Inventory>(createInventoryDto)).Returns(inventory);
            _inventoryRepositoryMock.Setup(r => r.CreateInventoryAsync(inventory)).Returns(Task.CompletedTask);
            _mapperMock.Setup(m => m.Map<ListInventoryDto>(inventory)).Returns(listInventoryDto);

            // Act
            var result = await _inventoryService.CreateInventoryAsync(createInventoryDto);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.That(okResult, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(okResult.Value, Is.Not.Null);
                Assert.That(listInventoryDto, Is.EqualTo(okResult.Value));
            });
        }

        [Test]
        public async Task GetAllInventoriesAsync_ReturnsOkObjectResult_WithListOfInventories()
        {
            // Arrange
            List<Inventory> inventories = [CreateArrange.CreateInventory(), CreateArrange.CreateInventory()];
            List<ListInventoryDto> inventoriesDto = [CreateArrange.CreateListInventoryDto(batches: []), CreateArrange.CreateListInventoryDto(batches: [])];
            PagedResultDto<ListInventoryDto> pagedResultDto = new(inventories.Count, inventoriesDto);

            //Config mock
            _inventoryRepositoryMock.Setup(r => r.GetAllInventoriesAsync(It.IsAny<int>(), It.IsAny<int>(), string.Empty, true)).ReturnsAsync((inventories.Count, inventories));
            _mapperMock.Setup(m => m.Map<IEnumerable<ListInventoryDto>>(inventories)).Returns(inventoriesDto);

            // Act
            var result = await _inventoryService.GetAllInventoriesAsync(skip: 0, take: 10, string.Empty, true);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.That(okResult, Is.Not.Null);
            var pagedReslt = okResult.Value as PagedResultDto<ListInventoryDto>;
            Assert.That(pagedReslt, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(pagedReslt.Records, Is.EqualTo(inventoriesDto));
                Assert.That(pagedReslt.TotalRecords, Is.EqualTo(inventoriesDto.Count));
            });
        }

        [Test]
        public async Task GetInventoryByIdAsync_ReturnsOkObjectResult_WhenInventoryExists()
        {
            // Arrange
            var inventory = CreateArrange.CreateInventory();
            var inventoryDto = CreateArrange.CreateListInventoryDto(batches: []);

            //Config mock
            _inventoryRepositoryMock.Setup(r => r.GetInventoryByIdAsync(1)).ReturnsAsync(inventory);
            _mapperMock.Setup(m => m.Map<ListInventoryDto>(inventory)).Returns(inventoryDto);

            // Act
            var result = await _inventoryService.GetInventoryByIdAsync(1);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.That(okResult, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(okResult.Value, Is.Not.Null);
                Assert.That(inventoryDto, Is.EqualTo(okResult.Value));
            });
        }

        [Test]
        public async Task GetInventoryByIdAsync_ReturnsNotFoundResult_WhenInventoryDoesNotExist()
        {
            // Arrange

            //Config mock
            _inventoryRepositoryMock.Setup(r => r.GetInventoryByIdAsync(1)).ReturnsAsync((Inventory?)null);

            // Act
            var result = await _inventoryService.GetInventoryByIdAsync(id: 1);

            // Assert
            Assert.That(result, Is.InstanceOf<NotFoundResult>());
        }

        [Test]
        public async Task ValidateInventoryParametersAsync_ReturnsOkResult_WhenParametersNotFound()
        {
            // Arrange
            Product product = new();
            NoteBase note = new()
            {
                Product = product
            };

            //Config mock
            _parametersRepositoryMock.Setup(r => r.GetParameterByInventoryIdAndProductIdAsync(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync((Parameters?)null);

            // Act
            var result = await _inventoryService.ValidateInventoryParametersAsync(note);

            // Assert
            Assert.That(result, Is.InstanceOf<OkResult>());
        }

        [Test]
        public async Task ValidateInventoryParametersAsync_CallsValidateOverrideAsync_AndReturnsOkResult()
        {
            // Arrange
            var product = new Product();
            var note = new NoteBase { Product = product };
            var parameters = new Parameters();

            //Mock config
            _parametersRepositoryMock.Setup(r => r.GetParameterByInventoryIdAndProductIdAsync(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(parameters);
            _overrideService.Setup(s => s.ValidateOverrideAsync(It.IsAny<NoteBase>(), It.IsAny<double>(), It.IsAny<Parameters>())).Returns(Task.CompletedTask);
            _batchRepositoryMock.Setup(r => r.GetAvaliableBatchesByInventoryIdAndProductIdAsync(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(new List<Batch>());

            // Act
            var result = await _inventoryService.ValidateInventoryParametersAsync(note);

            // Assert
            Assert.That(result, Is.InstanceOf<OkResult>());

            //Methods calls
            _overrideService.Verify(s => s.ValidateOverrideAsync(It.IsAny<NoteBase>(), It.IsAny<double>(), It.IsAny<Parameters>()), Times.AtLeastOnce());
        }

        [Test]
        public async Task GetTotalAvailableQuantityByInventoryIdAndProductIdAsync_ReturnsTotalQuantity()
        {
            // Arrange
            var inventoryId = 1;
            var productId = 1;
            var batches = new List<Batch>
            {
                CreateArrange.CreateBatch(inventoryId: inventoryId, productId: productId, quantity:5.0, expiryDate:DateTime.Now.AddDays(10)),
                CreateArrange.CreateBatch(inventoryId: inventoryId, productId: productId, quantity:10.0, expiryDate:DateTime.Now.AddDays(5)),
                CreateArrange.CreateBatch(inventoryId: inventoryId, productId: productId, quantity:3.0, expiryDate:null),
            };

            _batchRepositoryMock.Setup(r => r.GetAvaliableBatchesByInventoryIdAndProductIdAsync(inventoryId, productId))
                .ReturnsAsync(batches);

            // Act
            var result = await _inventoryService.GetTotalAvailableQuantityByInventoryIdAndProductIdAsync(inventoryId, productId);

            // Assert
            Assert.That(result, Is.EqualTo(18.0));
            _batchRepositoryMock.Verify(r => r.GetAvaliableBatchesByInventoryIdAndProductIdAsync(inventoryId, productId), Times.Once);
        }

        [Test]
        public async Task GetTotalAvailableQuantityByInventoryIdAndProductIdAsync_NoBatches_ReturnsZero()
        {
            // Arrange
            var inventoryId = 1;
            var productId = 1;
            var batches = new List<Batch>();

            _batchRepositoryMock.Setup(r => r.GetAvaliableBatchesByInventoryIdAndProductIdAsync(inventoryId, productId))
                .ReturnsAsync(batches);

            // Act
            var result = await _inventoryService.GetTotalAvailableQuantityByInventoryIdAndProductIdAsync(inventoryId, productId);

            // Assert
            Assert.That(result, Is.EqualTo(0));
            _batchRepositoryMock.Verify(r => r.GetAvaliableBatchesByInventoryIdAndProductIdAsync(inventoryId, productId), Times.Once);
        }
    }
}
