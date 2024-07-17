using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Moq;
using StockApp.API.Context;
using StockApp.API.Exceptions;
using StockApp.API.Repositories.Interfaces;
using StockApp.API.Services;
using StockApp.API.Services.Interfaces;
using StockApp.Models.Dtos.Pagination;
using StockApp.Models.Dtos.ProductDto;
using StockApp.Models.Dtos.SubCategoryDto;
using StockApp.Models.Enum;
using StockApp.Models.Models;

namespace StockApp.Tests.Services;

[TestFixture]
public class ProductServiceTest
{
    private Mock<IProductRepository> _productRepositoryMock;
    private Mock<IProductSubCategoryService> _productSubCategoryServiceMock;
    private Mock<IProductSubCategoryRepository> _productSubCategoryRepositoryMock;
    private Mock<IMapper> _mapperMock;
    private ProductService _productService;

    [SetUp]
    public void Setup()
    {
        _productRepositoryMock = new Mock<IProductRepository>();
        _productSubCategoryServiceMock = new Mock<IProductSubCategoryService>();
        _productSubCategoryRepositoryMock = new Mock<IProductSubCategoryRepository>();
        _mapperMock = new Mock<IMapper>();

        _productService = new ProductService(
            _mapperMock.Object, 
            _productRepositoryMock.Object, 
            _productSubCategoryServiceMock.Object, 
            _productSubCategoryRepositoryMock.Object);
    }

    [Test]
    public async Task GetAllProducts_ReturnsPagedResult()
    {
        // Arrange
        var products = new List<Product> 
        {
            new Product { Id = 1, Name = "Product1" },
            new Product { Id = 2, Name = "Product2" }
        };

        _productRepositoryMock
            .Setup(repo => repo.GetAllProducts(It.IsAny<int>(), It.IsAny<int>(), String.Empty, true))
            .ReturnsAsync((2, products));

        // Simulate mapping
        _mapperMock
            .Setup(mapper => mapper.Map<Product, ListProductDTO>(It.IsAny<Product>()))
            .Returns(new ListProductDTO());

        // Act
        var result = await _productService.GetAllProducts(0, 10, String.Empty, true);

        // Assert
        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);

        var pagedResult = okResult.Value as PagedResultDto<ListProductDTO>;
        Assert.IsNotNull(pagedResult);
        Assert.That(pagedResult.TotalRecords, Is.EqualTo(2));
    }
    
    [Test(Description = "Tests if the GetAllProducts returns all registered products, in this case should return 0")]
    public async Task GetAllProducts_ReturnsAnEmptyArrayOfProducts()
    {
        int skip = 0;
        int take = 10;
        string orderBy = "Id";
        bool isAscending = true;
        // Arrange
        var products = new List<Product>();

        _productRepositoryMock.Setup(repo => repo.GetAllProducts(skip,take, orderBy, isAscending)).ReturnsAsync((products.Count,products));
        
        // Act
        var result = await _productService.GetAllProducts(skip, take, orderBy, isAscending) as OkObjectResult;
        
        // Assert
        var productDtos = result?.Value as PagedResultDto<ListProductDTO>;
        Assert.That(productDtos?.Records.Count(), Is.EqualTo(0));
    }

    [Test(Description = "Get a product by ID and returns an OkObjectResult with the Product")]
    public async Task GetProductById_ReturnsAProduct()
    {
        var product = new Product()
        {
            Id = 1,
            Name = "Product 1",
            Category = CategoryEnum.Fruits
        };

        var productDto = new ListProductDTO()
        {
            Id = 1,
            Name = "Product 1",
            Category = CategoryEnum.Fruits
        };

        _productRepositoryMock.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(product);
        _mapperMock.Setup(map => map.Map<Product, ListProductDTO>(product)).Returns(productDto);

        // Act
        var result = await _productService.GetProductById(1) as OkObjectResult;
        
        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result.Value, Is.Not.Null);
            Assert.That(productDto, Is.EqualTo(result.Value));
        });
    }

    [Test]
    public async Task GetProductById_ThrowsNotFoundException()
    {
        // Arrange
        _productRepositoryMock.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync((Product)null);
        
        // Act
        var result = await _productService.GetProductById(1);
        
        // Assert
        Assert.That(result, Is.InstanceOf<NotFoundResult>());
    }

    [Test]
    public async Task CreateProduct_ReturnsOkObjectResult_WhenProductIsCreatedSuccessfully()
    {
        // Arrange
        var productDto = new CreateProductDto
        {
            Name = "Product 1",
            Category = CategoryEnum.Fruits
        };

        var product = new Product
        {
            Name = "Product 1",
            Category = CategoryEnum.Fruits
        };

        var createdProduct = new Product
        {
            Id = 1,
            Name = "Product 1",
            Category = CategoryEnum.Fruits
        };

        var listProductDto = new ListProductDTO
        {
            Id = 1,
            Name = "Product 1",
            Category = CategoryEnum.Fruits
        };

        _mapperMock.Setup(m => m.Map<CreateProductDto, Product>(productDto)).Returns(product);
        _productRepositoryMock.Setup(r => r.CreateProduct(product)).ReturnsAsync(createdProduct);
        _mapperMock.Setup(m => m.Map<Product, ListProductDTO>(createdProduct)).Returns(listProductDto);
        _productSubCategoryRepositoryMock.Setup(r => r.GetSubCategoriesByProductId(It.IsAny<int>())).Returns(new List<SubCategory>());
        _mapperMock.Setup(m => m.Map<List<ListSubCategoryDto>>(It.IsAny<List<SubCategory>>())).Returns(new List<ListSubCategoryDto>());

        // Act
        var result = await _productService.CreateProduct(productDto);

        // Assert
        Assert.IsInstanceOf<OkObjectResult>(result);
        var okResult = result as OkObjectResult;
        Assert.That(okResult.Value, Is.EqualTo(listProductDto));
    }
    
    [Test]
    public async Task CreateProduct_ReturnsBadRequestObjectResult_WhenSubCategoryDoesntExistExceptionIsThrown()
    {
        // Arrange
        var productDto = new CreateProductDto
        {
            Name = "Product 1",
            Category = CategoryEnum.Fruits
        };

        var product = new Product
        {
            Name = "Product 1",
            Category = CategoryEnum.Fruits
        };

        _mapperMock.Setup(m => m.Map<CreateProductDto, Product>(productDto)).Returns(product);
        _productRepositoryMock.Setup(r => r.CreateProduct(product)).ReturnsAsync(product);
        _productSubCategoryServiceMock
            .Setup(s => s.CreateProductSubCategory(It.IsAny<Product>(), It.IsAny<List<int>>()))
            .Throws(new SubCategoryDoesntExist("Subcategory doesn't exist"));

        // Act
        var result = await _productService.CreateProduct(productDto);

        // Assert
        Assert.IsInstanceOf<BadRequestObjectResult>(result);
        var badRequestResult = result as BadRequestObjectResult;
        Assert.That(badRequestResult.Value, Is.EqualTo("Subcategory doesn't exist"));
    }
}