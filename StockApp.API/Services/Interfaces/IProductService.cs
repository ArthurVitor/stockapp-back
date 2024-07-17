using Microsoft.AspNetCore.Mvc;
using StockApp.Models.Dtos.ProductDto;

namespace StockApp.API.Services.Interfaces;

public interface IProductService
{
    Task<IActionResult> GetAllProducts(int skip, int take, string orderBy, bool isAscending);

    Task<IActionResult> GetProductById(int id);

    Task<IActionResult> CreateProduct(CreateProductDto productDto);
    
    Task<IActionResult> DeleteProductById(int id);
}