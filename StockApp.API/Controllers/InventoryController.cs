using Microsoft.AspNetCore.Mvc;
using StockApp.API.Services.Interfaces;
using StockApp.Models.Dtos.InventoryDto;

namespace StockApp.API.Controllers;


[ApiController]
[Route("api/[controller]")]
public class InventoryController : ControllerBase
{
    private readonly IInventoryService _inventoryService;

    public InventoryController(IInventoryService inventoryService)
    {
        _inventoryService = inventoryService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateInventoryAsync([FromBody] CreateInventoryDto createInventoryDto)
    {
        return await _inventoryService.CreateInventoryAsync(createInventoryDto);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllInventoriesAsync([FromQuery] int skip = 0, [FromQuery] int take = 50, [FromQuery] string orderBy = "id", [FromQuery] bool isAscending = true)
    {
        return await _inventoryService.GetAllInventoriesAsync(skip, take, orderBy, isAscending);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetInventoryByIdAsync(int id)
    {
        return await _inventoryService.GetInventoryByIdAsync(id);
    }

    [HttpGet("product/{productId}/inventory/{inventoryId}/quantity")]
    public async Task<IActionResult> GetTotalProductQuantityInInventory(int productId, int inventoryId)
    {
        var totalQuantity = await _inventoryService.GetTotalAvailableQuantityByInventoryIdAndProductIdAsync(inventoryId, productId);
        return Ok(totalQuantity);
    }
}
