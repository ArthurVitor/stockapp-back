using Microsoft.EntityFrameworkCore;
using StockApp.API.Context;
using StockApp.API.Extensions;
using StockApp.API.Repositories.Interfaces;
using StockApp.Models.Models;

namespace StockApp.API.Repositories;

public class InventoryRepository : IInventoryRepository
{
    public AppDbContext _context;

    public InventoryRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task CreateInventoryAsync(Inventory inventory)
    {
        await _context.Inventories.AddAsync(inventory);
        await _context.SaveChangesAsync();
    }

    public async Task<(int totalRecords, IEnumerable<Inventory> records)> GetAllInventoriesAsync(int skip, int take, string orderBy, bool isAscending)
    {
        var totalRecords = await _context.Inventories.CountAsync();
        var intentories = await _context.Inventories
            .AsQueryable().OrderByField(orderBy, isAscending)
            .Skip(skip)
            .Take(take)
            .Include(i => i.Batches.Where(b => !b.IsUsed))
            .ToListAsync();
        return (totalRecords, intentories);
    }

    public async Task<Inventory> GetInventoryByIdAsync(int id)
    {
        return await _context.Inventories
        .Include(i => i.Batches.Where(b => !b.IsUsed))
        .FirstOrDefaultAsync(i => i.Id == id);
    }
}
