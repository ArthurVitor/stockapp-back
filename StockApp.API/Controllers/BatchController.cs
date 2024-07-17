using Microsoft.AspNetCore.Mvc;
using StockApp.API.Services.Interfaces;

namespace StockApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BatchController : ControllerBase
{
    private readonly IBatchService _batchService;

    public BatchController(IBatchService batchService)
    {
        _batchService = batchService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllAvaliableBatchesAsync([FromQuery] int skip = 0, [FromQuery] int take = 10, [FromQuery] string orderBy = "id", [FromQuery] bool isAscending = true)
    {
        return await _batchService.GetAllAvaliableBatchesAsync(skip, take, orderBy, isAscending);
    }

    [HttpGet("avaliable/{id}")]
    public async Task<IActionResult> GetAvaliableBatchByIdAsync(int id)
    {
        return await _batchService.GetAvaliableBatchByIdAsync(id);
    }

    [HttpGet("expired/{id}")]
    public async Task<IActionResult> GetExpiredBatchByIdAsync(int id)
    {
        return await _batchService.GetExpiredBatchByIdAsync(id);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteBatchAsync(int id)
    {
        return await _batchService.DeleteBatchByIdAsync(id);
    }
}
