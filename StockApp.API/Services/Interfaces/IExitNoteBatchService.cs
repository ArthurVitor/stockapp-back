using Microsoft.AspNetCore.Mvc;
using StockApp.Models.Models;

namespace StockApp.API.Services.Interfaces;

public interface IExitNoteBatchService
{
    public Task<IActionResult> CreateExitNoteBatchAsync(ExitNote exitNote, List<UsedBatch> usedBatches);
}
