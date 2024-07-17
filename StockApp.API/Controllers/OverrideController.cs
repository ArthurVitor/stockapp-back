using Microsoft.AspNetCore.Mvc;
using StockApp.API.Services;
using StockApp.API.Services.Interfaces;

namespace StockApp.API.Controllers;


[ApiController]
[Route("api/[controller]")]
public class OverrideController : ControllerBase
{
    private readonly IOverrideService _overrideService;

    public OverrideController(IOverrideService overrideService)
    {
        _overrideService = overrideService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllOverridesAsync([FromQuery] int skip = 0, [FromQuery] int take = 10, [FromQuery] string orderBy = "", [FromQuery] bool isAscending = true)
    {
        return await _overrideService.GetAllOverridesAsync(skip, take, orderBy, isAscending);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetOverrideByIdAsync(int id)
    {
        return await _overrideService.GetOverrideByIdAsync(id);
    }
}
