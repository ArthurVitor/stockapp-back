using StockApp.Models.Models;

namespace StockApp.API.Repositories.Interfaces;

public interface ISubCategoryRepository
{
    Task<(int totalRecords, IEnumerable<SubCategory> records)> GetAllSubCategoriesAsync(int skip, int take, string orderBy, bool isAscending);

    Task<SubCategory> GetSubCategoryByIdAsync(int id);

    Task CreateSubCategoryAsync(SubCategory subCategory);

    void BulkCreateSubCategories(IEnumerable<SubCategory> subCategories);

    List<int> BulkGetSubCategoryIdByName(ICollection<string> name);
}
