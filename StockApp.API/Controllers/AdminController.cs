using Microsoft.AspNetCore.Mvc;
using StockApp.API.Services.Interfaces;
using StockApp.Models.Dtos.AdminRoute;
using StockApp.Models.Dtos.Transactions;

namespace StockApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AdminController
{
    private readonly IAdminRouteService _adminRouteService;

    public AdminController(IAdminRouteService adminRouteService)
    {
        _adminRouteService = adminRouteService;
    }

    [HttpPost]
    [Route("reallocateBatch")]
    public async Task<IActionResult> ReallocateBatch(CreateReallocateProductDto createReallocateProductDto)
    {
        return await _adminRouteService.ReallocateBatch(createReallocateProductDto);
    }

    [HttpPost("reverse-transaction")]
    public async Task<IActionResult> ReverseTransaction([FromBody] ReverseTransactionDto reverseTransactionDto)
    {
        return await _adminRouteService.ReverseTransactionAsync(reverseTransactionDto);
    }

    [HttpGet]
    [Route("getAllPerishedBatches")]
    public async Task<IActionResult> GetAllPerishedBatches([FromQuery] int skip = 0, [FromQuery] int take = 10, [FromQuery] string orderBy = "id", [FromQuery] bool isAscending = true)
    {
        return await _adminRouteService.GetAllExpiredBatches(skip, take, orderBy, isAscending);
    }

    [HttpPost]  
    [Route("deleteAllPerishedBatches")]
    public async Task<IActionResult> DeleteAllPerishedBatches()
    {
        return await _adminRouteService.DeleteAllPerishedBatches();
    }
}