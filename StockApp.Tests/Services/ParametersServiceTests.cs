using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Moq;
using StockApp.API.Repositories.Interfaces;
using StockApp.API.Services;
using StockApp.Models.Dtos.InventoryDto;
using StockApp.Models.Dtos.Pagination;
using StockApp.Models.Dtos.Parameters;
using StockApp.Models.Models;
using StockApp.Tests.Utils;

namespace StockApp.Tests.Services;

[TestFixture]
public class ParametersServiceTest
{
    private Mock<IMapper> _mapperMock;
    private Mock<IParametersRepository> _parametersRepositoryMock;
    private Mock<IProductRepository> _productRepositoryMock;
    private Mock<IInventoryRepository> _inventoryRepositoryMock;
    private ParameterService _parameterService;

    [SetUp]
    public void SetUp()
    {
        //Mocks creation
        _mapperMock = new Mock<IMapper>();
        _parametersRepositoryMock = new Mock<IParametersRepository>();
        _productRepositoryMock = new Mock<IProductRepository>();
        _inventoryRepositoryMock = new Mock<IInventoryRepository>();

        // Services creation
        _parameterService = new ParameterService(
            _mapperMock.Object,
            _parametersRepositoryMock.Object,
            _productRepositoryMock.Object,
            _inventoryRepositoryMock.Object
            );
    }

    [Test]
    public async Task CreateParameter_ReturnsOkObjectResult_WhenSuccessful()
    {
        // Arrange
        var inventory = CreateArrange.CreateInventory();
        var parameters = CreateArrange.CreateParameter();
        var createParameterDto = CreateArrange.CreateParameterDto();
        var listParameterDto = CreateArrange.CreateListParameterDto();

        // Config mocks
        _mapperMock.Setup(m => m.Map<Parameters>(createParameterDto)).Returns(parameters);
        _mapperMock.Setup(m => m.Map<ListParameterDto>(parameters)).Returns(listParameterDto);
        _parametersRepositoryMock.Setup(r => r.CreateProductParameterAsync(parameters)).ReturnsAsync(parameters);
        _parametersRepositoryMock.Setup(r => r.GetParameterByInventoryIdAndProductIdAsync(parameters.InventoryId, parameters.ProductId)).ReturnsAsync((Parameters?)null);
        _productRepositoryMock.Setup(r => r.Exists(parameters.ProductId)).Returns(true);
        _inventoryRepositoryMock.Setup(r => r.GetInventoryByIdAsync(parameters.InventoryId)).ReturnsAsync(inventory);

        // Act
        var result = await _parameterService.CreateParameter(createParameterDto);

        // Assert
        var okResult = result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(okResult.StatusCode, Is.EqualTo(200));
            Assert.That(okResult.Value, Is.EqualTo(listParameterDto));
        });
    }

    [Test]
    public async Task CreateParameter_ReturnsNotFoundObjectResult_WhenProductDoesNotExist()
    {
        // Arrange
        var parameters = CreateArrange.CreateParameter();
        var createParameterDto = CreateArrange.CreateParameterDto();

        // Config mocks
        _mapperMock.Setup(m => m.Map<Parameters>(createParameterDto)).Returns(parameters);
        _productRepositoryMock.Setup(r => r.Exists(parameters.ProductId)).Returns(false);

        // Act
        var result = await _parameterService.CreateParameter(createParameterDto);

        // Assert
        var notFoundResult = result as NotFoundObjectResult;
        Assert.That(notFoundResult, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(notFoundResult.StatusCode, Is.EqualTo(404));
            Assert.That(notFoundResult.Value, Is.EqualTo($"Product with Id {parameters.ProductId} doesn't exist."));
        });
    }

    [Test]
    public async Task CreateParameter_ReturnsNotFoundObjectResult_WhenInventoryDoesNotExist()
    {
        // Arrange
        var parameters = CreateArrange.CreateParameter();
        var createParameterDto = CreateArrange.CreateParameterDto();

        // Config mocks
        _mapperMock.Setup(m => m.Map<Parameters>(createParameterDto)).Returns(parameters);
        _productRepositoryMock.Setup(r => r.Exists(parameters.ProductId)).Returns(true);
        _inventoryRepositoryMock.Setup(r => r.GetInventoryByIdAsync(parameters.InventoryId)).ReturnsAsync((Inventory)null);

        // Act
        var result = await _parameterService.CreateParameter(createParameterDto);

        // Assert
        var notFoundResult = result as NotFoundObjectResult;
        Assert.That(notFoundResult, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(notFoundResult.StatusCode, Is.EqualTo(404));
            Assert.That(notFoundResult.Value, Is.EqualTo($"Inventory with Id {parameters.InventoryId} doesn't exist."));
        });
    }

    [Test]
    public async Task CreateParameter_ReturnsBadRequestObjectResult_WhenParametersAlreadyExist()
    {
        // Arrange
        var parameters = CreateArrange.CreateParameter();
        var createParameterDto = CreateArrange.CreateParameterDto();

        // Config mocks
        _mapperMock.Setup(m => m.Map<Parameters>(createParameterDto)).Returns(parameters);
        _productRepositoryMock.Setup(r => r.Exists(parameters.ProductId)).Returns(true);
        _inventoryRepositoryMock.Setup(r => r.GetInventoryByIdAsync(parameters.InventoryId)).ReturnsAsync(new Inventory());
        _parametersRepositoryMock.Setup(r => r.GetParameterByInventoryIdAndProductIdAsync(parameters.InventoryId, parameters.ProductId)).ReturnsAsync(parameters);

        // Act
        var result = await _parameterService.CreateParameter(createParameterDto);

        // Assert
        var badRequestResult = result as BadRequestObjectResult;
        Assert.That(badRequestResult, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(badRequestResult.StatusCode, Is.EqualTo(400));
            Assert.That(badRequestResult.Value, Is.EqualTo($"A parameter with ProductId {parameters.ProductId} and InventoryId {parameters.InventoryId} already exists."));
        });
    }

    [Test]
    public async Task GetAllParameters_ReturnsOkObjectResult_WithListOfParameterDtos()
    {
        // Arrange
        var parameters = new List<Parameters>
            {
                CreateArrange.CreateParameter(),
                CreateArrange.CreateParameter(),
            };

        var listParameterDtos = new List<ListParameterDto>
            {
                CreateArrange.CreateListParameterDto(productId:1, inventoryId:1, maximumAmount:100, minimumAmount:10),
                CreateArrange.CreateListParameterDto(productId:2, inventoryId:2, maximumAmount:200, minimumAmount:20),
            };
        int skip = 0;
        int take = 10;
        string orderBy = string.Empty;
        bool isAscending = true;
        
        // Config mocks
        _parametersRepositoryMock.Setup(r => r.GetAllParametersAsync(skip, take, orderBy, isAscending)).ReturnsAsync((parameters.Count,parameters));
        _mapperMock.Setup(m => m.Map<ListParameterDto>(parameters[0])).Returns(listParameterDtos[0]);
        _mapperMock.Setup(m => m.Map<ListParameterDto>(parameters[1])).Returns(listParameterDtos[1]);

        // Act
        var result = await _parameterService.GetAllParameters(skip, take, orderBy, isAscending);

        // Assert
        var okResult = result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);
        var pagedReslt = okResult.Value as PagedResultDto<ListParameterDto>;
        Assert.That(pagedReslt, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(pagedReslt.Records, Is.EqualTo(listParameterDtos));
            Assert.That(pagedReslt.TotalRecords, Is.EqualTo(listParameterDtos.Count));
        });
    }

    [Test]
    public async Task GetParameterById_ReturnsOkObjectResult_WhenParameterExists()
    {
        // Arrange
        var parameter = CreateArrange.CreateParameter();
        var listParameterDto = CreateArrange.CreateListParameterDto();

        // Config mocks
        _parametersRepositoryMock.Setup(r => r.GetParameterByIdAsync(1)).ReturnsAsync(parameter);
        _mapperMock.Setup(m => m.Map<ListParameterDto>(parameter)).Returns(listParameterDto);

        // Act
        var result = await _parameterService.GetParameterById(id: 1);

        // Assert
        var okResult = result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(okResult.StatusCode, Is.EqualTo(200));
            Assert.That(okResult.Value, Is.EqualTo(listParameterDto));
        });
    }

    [Test]
    public async Task GetParameterById_ReturnsNotFoundObjectResult_WhenParameterDoesNotExist()
    {
        // Arrange
        var notFoundId = 1;

        // Config mocks
        _parametersRepositoryMock.Setup(r => r.GetParameterByIdAsync(notFoundId)).ReturnsAsync((Parameters?)null);

        // Act
        var result = await _parameterService.GetParameterById(id: notFoundId);

        // Assert
        var notFoundResult = result as NotFoundObjectResult;
        Assert.That(notFoundResult, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(notFoundResult.StatusCode, Is.EqualTo(404));
            Assert.That(notFoundResult.Value, Is.EqualTo($"Parameter with Id: {notFoundId} doesn't exist"));
        });
    }

    [Test]
    public async Task GetParameterByInventoryIdAndProductIdAsync_ParameterExists_ReturnsOkObjectResult()
    {
        // Arrange
        var inventoryId = 1;
        var productId = 1;
        var parameter = CreateArrange.CreateParameter(inventoryId, productId);
        var parameterDto = CreateArrange.CreateListParameterDto(productId, inventoryId);

        //Config mock
        _parametersRepositoryMock.Setup(r => r.GetParameterByInventoryIdAndProductIdAsync(inventoryId, productId))
            .ReturnsAsync(parameter);
        _mapperMock.Setup(m => m.Map<ListParameterDto>(parameter))
            .Returns(parameterDto);

        // Act
        var result = await _parameterService.GetParameterByInventoryIdAndProductIdAsync(inventoryId, productId);

        // Assert
        var okResult = result as OkObjectResult;
        Assert.That(okResult,  Is.Not.Null);
        Assert.That(okResult.Value, Is.EqualTo(parameterDto)); 
        _parametersRepositoryMock.Verify(r => r.GetParameterByInventoryIdAndProductIdAsync(inventoryId, productId), Times.Once);
        _mapperMock.Verify(m => m.Map<ListParameterDto>(parameter), Times.Once);
    }

    [Test]
    public async Task GetParameterByInventoryIdAndProductIdAsync_ParameterDoesNotExist_ReturnsNotFoundObjectResult()
    {
        // Arrange
        var inventoryId = 1;
        var productId = 1;

        _parametersRepositoryMock.Setup(r => r.GetParameterByInventoryIdAndProductIdAsync(inventoryId, productId))
            .ReturnsAsync((Parameters)null);

        // Act
        var result = await _parameterService.GetParameterByInventoryIdAndProductIdAsync(inventoryId, productId);

        // Assert
        var notFoundResult = result as NotFoundObjectResult;
        Assert.That(notFoundResult, Is.Not.Null);
        Assert.That(notFoundResult.Value, Is.EqualTo($"Parameter with inventory id {inventoryId} and product id {productId} doesn't exist"));

        //verify mock calls
        _parametersRepositoryMock.Verify(r => r.GetParameterByInventoryIdAndProductIdAsync(inventoryId, productId), Times.Once);
        _mapperMock.Verify(m => m.Map<ListParameterDto>(It.IsAny<Parameters>()), Times.Never);
    }
}
