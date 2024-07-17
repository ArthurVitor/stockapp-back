using Microsoft.AspNetCore.Mvc;
using StockApp.API.Services.Interfaces;
using StockApp.Models.Dtos.SubCategoryDto;

namespace StockApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SubCategoriesController : ControllerBase
{
    private readonly ISubCategoryService _subCategoryService;

    public SubCategoriesController(ISubCategoryService subCategoryService)
    {
        _subCategoryService = subCategoryService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllSubCategoriesAsync([FromQuery] int skip = 0, [FromQuery] int take = 10, [FromQuery] string orderBy = "Id", [FromQuery] bool isAscending = true)
    {
        return await _subCategoryService.GetAllSubCategoriesAsync(skip, take, orderBy, isAscending);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetSubCategoryByIdAsync(int id)
    {
       return await _subCategoryService.GetSubCategoryByIdAsync(id);
    }

    [HttpPost]
    public async Task<IActionResult> CreateSubCategoryAsync([FromBody] CreateSubCategoryDto subCategoryDto)
    {
        return await _subCategoryService.CreateSubCategoryAsync(subCategoryDto);
    }
}
