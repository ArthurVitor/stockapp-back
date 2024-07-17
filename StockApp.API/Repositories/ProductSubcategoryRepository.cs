using StockApp.API.Context;
using StockApp.API.Repositories.Interfaces;
using StockApp.Models.Models;

namespace StockApp.API.Repositories;

public class ProductSubcategoryRepository : IProductSubCategoryRepository
{
    private readonly AppDbContext _context;

    public ProductSubcategoryRepository(AppDbContext context)
    {
        _context = context;
    }

    public Task<ProductSubCategory> CreateProductSubcategory(ProductSubCategory productSubCategory)
    {
        _context.ProductSubCategories.Add(productSubCategory);
        _context.SaveChangesAsync();
        
        return Task.FromResult(productSubCategory);
    }
    
    public void CreateProductSubcategories(IEnumerable<ProductSubCategory> productSubCategories)
    {
        _context.AddRange(productSubCategories);
        _context.SaveChanges();
    }

    public IEnumerable<SubCategory> GetSubCategoriesByProductId(int id)
    {
        var subCategories = _context.ProductSubCategories
            .Where(p => p.ProductId == id)
            .Select(p => p.SubCategory)
            .ToList();

        return subCategories;
    }
}