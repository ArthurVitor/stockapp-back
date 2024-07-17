using Microsoft.AspNetCore.Mvc;
using StockApp.API.Services.Interfaces;
using StockApp.Models.Dtos.Parameters;

namespace StockApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ParametersController
{
    private readonly IParameterService _parameterService;

    public ParametersController(IParameterService parameterService)
    {
        _parameterService = parameterService;
    }

    [HttpPost]
    public Task<IActionResult> CreateParameter(CreateParameterDto parameterDto)
    {
        return _parameterService.CreateParameter(parameterDto);
    }

    [HttpGet]
    public Task<IActionResult> GetAllParameters([FromQuery] int skip = 0, [FromQuery] int take = 10, [FromQuery] string orderBy = "id", [FromQuery] bool isAscending = true)
    {
        return _parameterService.GetAllParameters(skip, take, orderBy, isAscending);
    }
    
    [HttpGet("{id}")]
    public Task<IActionResult> GetParameterById(int id)
    {
        return _parameterService.GetParameterById(id);
    }

    [HttpGet("byInventoryAndProduct")]
    public Task<IActionResult> GetParameterByInventoryIdAndProductId([FromQuery] int inventoryId, [FromQuery] int productId)
    {
        return _parameterService.GetParameterByInventoryIdAndProductIdAsync(inventoryId, productId);
    }

}