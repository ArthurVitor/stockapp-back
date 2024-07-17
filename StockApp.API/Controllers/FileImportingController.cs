using Microsoft.AspNetCore.Mvc;
using StockApp.API.Services.Interfaces;

namespace StockApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FileImportingController : ControllerBase
{
    private readonly IImportFileService _fileService;

    public FileImportingController(IImportFileService fileService)
    {
        _fileService = fileService;
    }
    
    [HttpPost("upload/subcategory")]
    public async Task<IActionResult> SubCategoryImporting(IFormFile file)
    {
        return await _fileService.ImportFileSubCategory(file);
    }

    [HttpPost("upload/product")]
    public async Task<IActionResult> ProductImporting(IFormFile file)
    {
        return await _fileService.ImportFileProduct(file);
    }
}