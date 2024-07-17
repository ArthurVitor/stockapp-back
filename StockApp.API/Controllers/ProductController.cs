using Microsoft.AspNetCore.Mvc;
using StockApp.API.Services.Interfaces;
using StockApp.Models.Dtos.ProductDto;

namespace StockApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductController : ControllerBase
{
    private readonly IProductService _productService;
    
    public ProductController(IProductService productService)
    {
        _productService = productService;
    }

    [HttpGet]
    public Task<IActionResult> GetAllProducts([FromQuery] int skip = 0, [FromQuery] int take = 10, [FromQuery] string orderBy = "", [FromQuery] bool isAscending = true)
    {
        return _productService.GetAllProducts(skip, take, orderBy, isAscending);
    }

    [HttpGet("{id}")]
    public Task<IActionResult> GetProductById(int id)
    {
        return _productService.GetProductById(id);
    }

    [HttpPost]
    public Task<IActionResult> CreateProduct([FromBody] CreateProductDto productDto)
    {
        return _productService.CreateProduct(productDto);
    }
    
    [HttpDelete("{id}")]
    public Task<IActionResult> DeleteProductById(int id)
    {
        return _productService.DeleteProductById(id);
    }
}