using Microsoft.AspNetCore.Mvc;

namespace StockApp.API.Services.Interfaces;

public interface IImportFileService
{
    Task<IActionResult> ImportFileSubCategory(IFormFile file);
    Task<IActionResult> ImportFileProduct(IFormFile file);
}