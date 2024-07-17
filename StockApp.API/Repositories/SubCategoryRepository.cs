using Microsoft.EntityFrameworkCore;
using StockApp.API.Context;
using StockApp.API.Exceptions;
using StockApp.API.Extensions;
using StockApp.API.Repositories.Interfaces;
using StockApp.Models.Models;

namespace StockApp.API.Repositories;

public class SubCategoryRepository : ISubCategoryRepository
{

    public AppDbContext _context;

    public SubCategoryRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task CreateSubCategoryAsync(SubCategory subCategory)
    {
        await _context.SubCategories.AddAsync(subCategory);
        await _context.SaveChangesAsync();
    }

    public void BulkCreateSubCategories(IEnumerable<SubCategory> subCategories)
    {
        _context.SubCategories.AddRange(subCategories);
        _context.SaveChanges();
    }

    public List<int> BulkGetSubCategoryIdByName(ICollection<string> subCategoryNames)
    {
        List<int> subCategoriesId = new List<int>();

        foreach (var subCategoryName in subCategoryNames)
        {
            var subCategory = _context.SubCategories.FirstOrDefault(sc => sc.Name == subCategoryName);

            if (subCategory == null)
            {
                throw new SubCategoryDoesntExist($"SubCategory with name {subCategoryName} doesn't exist, try creating it before importing");
            }
            
            subCategoriesId.Add(subCategory.Id);
        }

        return subCategoriesId;
    }

    public async Task<(int totalRecords, IEnumerable<SubCategory> records)> GetAllSubCategoriesAsync(int skip, int take, string orderBy, bool isAscending)
    {
        var totalRecords = await _context.SubCategories.CountAsync();
        var records = await _context.SubCategories.AsQueryable().OrderByField(orderBy, isAscending).Skip(skip).Take(take).ToListAsync();
        return (totalRecords, records);
    }

    public async Task<SubCategory> GetSubCategoryByIdAsync(int id)
    {
        return await _context.SubCategories.FindAsync(id);
    }
}
