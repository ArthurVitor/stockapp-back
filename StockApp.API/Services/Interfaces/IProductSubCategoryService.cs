using StockApp.Models.Models;

namespace StockApp.API.Services.Interfaces;

public interface IProductSubCategoryService
{
    void CreateProductSubCategory(Product product, ICollection<int> subCategories);
}