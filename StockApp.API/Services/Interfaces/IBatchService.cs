using Microsoft.AspNetCore.Mvc;
using StockApp.Models.Models;

namespace StockApp.API.Services.Interfaces;

public interface IBatchService
{
    Task<Batch> CreateBatchAsync(EntryNote note);
    Task<IActionResult> GetAllAvaliableBatchesAsync(int skip, int take, string orderBy, bool isAscending);
    Task<IActionResult> GetAvaliableBatchByIdAsync(int id);
    Task<IActionResult> GetExpiredBatchByIdAsync(int id);
    Task<List<UsedBatch>> DeductQuantityFromBatchesAsync(IEnumerable<Batch> batches, double quantityToDeduct);
    Task<IActionResult> DeleteBatchByIdAsync(int id); 
}
