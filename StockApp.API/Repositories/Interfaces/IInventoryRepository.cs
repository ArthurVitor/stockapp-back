using StockApp.Models.Models;

namespace StockApp.API.Repositories.Interfaces;

public interface IInventoryRepository
{
    Task CreateInventoryAsync(Inventory inventory);

    Task<(int totalRecords, IEnumerable<Inventory> records)> GetAllInventoriesAsync(int skip, int take, string orderBy, bool isAscending);

    Task<Inventory> GetInventoryByIdAsync(int id);
}
