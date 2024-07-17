using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StockApp.API.Context;
using StockApp.API.Extensions;
using StockApp.API.Repositories.Interfaces;
using StockApp.Models.Models;

namespace StockApp.API.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly AppDbContext _context;

    public ProductRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<(int totalRecords, IEnumerable<Product> records)> GetAllProducts(int skip, int take, string orderBy = "", bool isAscending = true)
    {
        var totalRecords = await _context.Products.CountAsync();
        var products = await _context.Products.AsQueryable().OrderByField(orderBy, isAscending).Skip(skip).Take(take).ToListAsync();

        return (totalRecords, products);
    }

    public async Task<Product?> GetByIdAsync(int id)
    {
        var product = await _context.Products.FindAsync(id);
        return product;
    }

    public async Task<Product> CreateProduct(Product product)
    {
        _context.Add(product);
        await _context.SaveChangesAsync();

        return product;
    }

    public async Task<IActionResult> DeleteProductById(int id)
    {
        var product = await _context.Products.FindAsync(id);

        if (product is null) return new NotFoundResult();

        _context.Remove(product);
        await _context.SaveChangesAsync();

        return new NoContentResult();
    }

    public bool Exists(int id)
    {
        return _context.Products.Any(p => p.Id == id);
    }

    public void BulkCreateProduct(IEnumerable<Product> products)
    {
        _context.Products.AddRange(products);
        _context.SaveChanges();
    }
}