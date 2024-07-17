using Microsoft.AspNetCore.Mvc;
using StockApp.Models.Models;

namespace StockApp.API.Repositories.Interfaces;

public interface IProductRepository
{
    Task<(int totalRecords, IEnumerable<Product> records)> GetAllProducts(int skip, int take, string orderBy, bool isAscending);
    
    Task<Product?> GetByIdAsync(int id);

    Task<Product> CreateProduct(Product product);

    Task<IActionResult> DeleteProductById(int id);

    bool Exists(int id);

    void BulkCreateProduct(IEnumerable<Product> products);
}