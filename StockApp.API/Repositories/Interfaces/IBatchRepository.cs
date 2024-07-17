using StockApp.Models.Models;

namespace StockApp.API.Repositories.Interfaces;

public interface IBatchRepository
{
    Task<(int totalRecords, IEnumerable<Batch> records)> GetAllAvaliableBatchesAsync(int skip, int take, string orderBy, bool isAscending);
    Task<Batch> GetAvaliableBatchByIdAsync(int id);
    Task<Batch> GetExpiredBatchByIdAsync(int id); 
    Task CreateBatchAsync(Batch batch);
    Task<IEnumerable<Batch>> GetAvaliableBatchesByInventoryIdAndProductIdAsync(int inventoryId, int productId);
    Task DeleteBatchAsync(Batch batch);
    Task UpdateBatchAsync(Batch batch);
    Task<IEnumerable<Batch>> GetBatchesByIdsAsync(IEnumerable<int> batchIds);
    Task<Batch> GetBatchByIdAsync(int id); 
}
