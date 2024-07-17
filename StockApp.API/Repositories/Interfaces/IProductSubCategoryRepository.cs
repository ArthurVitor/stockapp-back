using StockApp.Models.Models;

namespace StockApp.API.Repositories.Interfaces;

public interface IProductSubCategoryRepository
{
    Task<ProductSubCategory> CreateProductSubcategory(ProductSubCategory productSubCategory);
    
    void CreateProductSubcategories(IEnumerable<ProductSubCategory> productSubCategories);

    IEnumerable<SubCategory> GetSubCategoriesByProductId(int id);
}