using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using StockApp.API.Exceptions;
using StockApp.API.Extensions;
using StockApp.API.Repositories.Interfaces;
using StockApp.API.Services.Interfaces;
using StockApp.Models.Dtos.Pagination;
using StockApp.Models.Dtos.ProductDto;
using StockApp.Models.Dtos.SubCategoryDto;
using StockApp.Models.Models;

namespace StockApp.API.Services;

public class ProductService : IProductService
{
    private readonly IMapper _mapper;

    private readonly IProductRepository _productRepository;

    private readonly IProductSubCategoryService _productSubCategoryService;

    private readonly IProductSubCategoryRepository _productSubCategoryRepository;

    public ProductService(IMapper mapper, IProductRepository productRepository, IProductSubCategoryService productSubCategoryService, IProductSubCategoryRepository productSubCategoryRepository)
    {
        _mapper = mapper;
        _productRepository = productRepository;
        _productSubCategoryService = productSubCategoryService;
        _productSubCategoryRepository = productSubCategoryRepository;
    }

    public async Task<IActionResult> GetAllProducts(int skip, int take, string orderBy = "id", bool isAscending = true)
    {
        var (totalRecords, products) = await _productRepository.GetAllProducts(skip, take, orderBy, isAscending);

        var productDtOs = products.Select(GenerateListProductDto);
        
        return new OkObjectResult(new PagedResultDto<ListProductDTO>(totalRecords, productDtOs));
    }

    public async Task<IActionResult> GetProductById(int id)
    {
        try
        {
            var product = await _productRepository.GetByIdAsync(id);

            if (product is null)
            {
                return new NotFoundResult();
            }

            var productDto = GenerateListProductDto(product);
            return new OkObjectResult(productDto);
        }
        catch
        {
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
    }

    public async Task<IActionResult> CreateProduct(CreateProductDto productDto)
    {
        try
        {
            var product = _mapper.Map<CreateProductDto, Product>(productDto);

            var createdProduct = await _productRepository.CreateProduct(product);

            _productSubCategoryService.CreateProductSubCategory(createdProduct, productDto.SubCategories);

            ListProductDTO listProductDto = GenerateListProductDto(createdProduct);

            return new OkObjectResult(listProductDto);
        }
        catch (SubCategoryDoesntExist e)
        {
            return new BadRequestObjectResult(e.Message);
        }
    }

    public Task<IActionResult> DeleteProductById(int id)
    {
        return _productRepository.DeleteProductById(id);
    }

    private ListProductDTO GenerateListProductDto(Product product)
    {
        var listProductDto = _mapper.Map<Product, ListProductDTO>(product);

        var subCategories = _productSubCategoryRepository.GetSubCategoriesByProductId(product.Id);

        listProductDto.SubCategories = _mapper.Map<List<ListSubCategoryDto>>(subCategories);

        return listProductDto;
    }
}