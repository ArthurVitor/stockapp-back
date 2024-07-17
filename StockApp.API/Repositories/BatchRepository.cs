using Microsoft.EntityFrameworkCore;
using StockApp.API.Context;
using StockApp.API.Extensions;
using StockApp.API.Repositories.Interfaces;
using StockApp.Models.Models;

namespace StockApp.API.Repositories;

public class BatchRepository : IBatchRepository
{
    public AppDbContext _context;

    public BatchRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task CreateBatchAsync(Batch batch)
    {
        await _context.Batches.AddAsync(batch);
        await _context.SaveChangesAsync();
    }

    public async Task<(int totalRecords, IEnumerable<Batch> records)> GetAllAvaliableBatchesAsync(int skip, int take, string orderBy, bool isAscending)
    {
        var currentDate = DateTime.Now;
        var totalRecords = await _context.Batches
            .CountAsync(b => !b.IsUsed && (b.ExpiryDate == null || b.ExpiryDate > currentDate));

        var records = await _context.Batches
            .AsQueryable()
            .OrderByField(orderBy, isAscending)
            .Where(b => !b.IsUsed && (b.ExpiryDate == null || b.ExpiryDate > currentDate))
            .Skip(skip)
            .Take(take)
            .ToListAsync();

        return (totalRecords, records);
    }

    public async Task<Batch> GetAvaliableBatchByIdAsync(int id)
    {
        var currentDate = DateTime.Now;
        var batch = await _context.Batches
            .Where(b => b.Id == id
                && !b.IsUsed && (b.ExpiryDate == null || b.ExpiryDate > currentDate))
            .FirstOrDefaultAsync();
        return batch;
    }

    public async Task<Batch> GetExpiredBatchByIdAsync(int id)
    {
        var currentDate = DateTime.Now;
        var batch = await _context.Batches
            .Where(b => b.Id == id && (b.ExpiryDate != null && b.ExpiryDate <= currentDate)
            && !b.IsUsed)
            .FirstOrDefaultAsync();
        return batch;
    }

    public async Task<IEnumerable<Batch>> GetAvaliableBatchesByInventoryIdAndProductIdAsync(int inventoryId, int productId)
    {
        var currentDate = DateTime.Now;
        return await _context.Batches
                                .Include(b => b.EntryNote)
                                .Where(b => b.InventoryId == inventoryId
                                && b.ProductId == productId
                                && !b.IsUsed
                                && (b.ExpiryDate == null || b.ExpiryDate > currentDate))
                                .ToListAsync();
    }

    public async Task DeleteBatchAsync(Batch batch)
    {
        batch.IsUsed = true;
        await UpdateBatchAsync(batch);
    }

    public async Task UpdateBatchAsync(Batch batch)
    {
        _context.Batches.Update(batch);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<Batch>> GetBatchesByIdsAsync(IEnumerable<int> batchIds)
    {
        return await _context.Batches
                             .Where(batch => batchIds.Contains(batch.Id))
                             .ToListAsync();
    }

    public async Task<Batch> GetBatchByIdAsync(int id)
    {
        var batch = await _context.Batches.FirstOrDefaultAsync(b => b.Id == id);
        return batch;
    }
}
