using Microsoft.AspNetCore.Mvc;
using StockApp.Models.Dtos.SubCategoryDto;

namespace StockApp.API.Services.Interfaces;

public interface ISubCategoryService
{
    Task<IActionResult> GetAllSubCategoriesAsync(int skip, int take, string orderBy, bool isAscending);

    Task<IActionResult> GetSubCategoryByIdAsync(int id);

    Task<IActionResult> CreateSubCategoryAsync(CreateSubCategoryDto subCategoryDto);
}
