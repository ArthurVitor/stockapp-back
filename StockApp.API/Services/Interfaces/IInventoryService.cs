using Microsoft.AspNetCore.Mvc;
using StockApp.Models.Dtos.InventoryDto;
using StockApp.Models.Models;

namespace StockApp.API.Services.Interfaces;

public interface IInventoryService
{
    Task<IActionResult> CreateInventoryAsync(CreateInventoryDto inventoryDto);

    Task<IActionResult> GetAllInventoriesAsync(int skip, int take, string orderBy, bool isAscending);

    Task<IActionResult> GetInventoryByIdAsync(int id);

    Task<(IEnumerable<Batch> batches, IActionResult validationResult)> ValidateAvailableBatchesAsync(int inventoryId, int productId, double requestedQuantity);

    Task<IActionResult> ValidateInventoryParametersAsync(NoteBase note);

    public Task<double> GetTotalAvailableQuantityByInventoryIdAndProductIdAsync(int inventoryId, int productId);
}
