using StockApp.API.Context;
using StockApp.API.Exceptions;
using StockApp.API.Repositories.Interfaces;
using StockApp.API.Services.Interfaces;
using StockApp.Models.Models;

namespace StockApp.API.Services;

public class ProductSubCategoryService : IProductSubCategoryService
{
    private readonly IProductSubCategoryRepository _productSubCategoryRepository;

    private readonly AppDbContext _context;
    
    public ProductSubCategoryService(IProductSubCategoryRepository productSubCategoryRepository, AppDbContext context)
    {
        _productSubCategoryRepository = productSubCategoryRepository;
        _context = context;
    }
    
    public void CreateProductSubCategory(Product product, ICollection<int> subCategories)
    {
        var missingIds = subCategories.Except(_context.SubCategories.Where(sc => subCategories.Contains(sc.Id)).Select(sc => sc.Id));

        if (missingIds.Any())
        {
            var missingIdList = string.Join(", ", missingIds);
            throw new SubCategoryDoesntExist($"Subcategories with IDs {missingIdList} don't exist.");
        }

        _productSubCategoryRepository.CreateProductSubcategories(subCategories.Select(subCategoryId => new ProductSubCategory
        {
            ProductId = product.Id,
            SubCategoryId = subCategoryId
        }));
    }
}