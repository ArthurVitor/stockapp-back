using Microsoft.EntityFrameworkCore;
using StockApp.API.Context;
using StockApp.API.Extensions;
using StockApp.API.Repositories.Interfaces;
using StockApp.Models.Models;

namespace StockApp.API.Repositories;

public class AdminRouteRepository : IAdminRouteRepository
{
    private readonly AppDbContext _context;

    public AdminRouteRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<(int totalRecords, IEnumerable<Batch> records)> GetAllExpiredBatches(int skip, int take, string orderBy, bool isAscending)
    {
        var currentDate = DateTime.Now;
        var totalRecords = await _context.Batches
              .Where(bt => bt.ExpiryDate < currentDate && !bt.IsUsed)
              .CountAsync();

        var expiredBatches = await _context.Batches
            .AsQueryable()
            .OrderByField(orderBy, isAscending)
             .Where(bt => bt.ExpiryDate < currentDate && !bt.IsUsed)
            .Skip(skip)
            .Take(take)
            .ToListAsync();

        return (totalRecords, expiredBatches);
    }

    public async Task<IEnumerable<Batch>> GetAllExpiredBatches()
    {
        var expiredBatches = await _context.Batches
            .Where(bt => bt.ExpiryDate < DateTime.Now && !bt.IsUsed)
            .ToListAsync();

        return expiredBatches;
    }

    public async Task BulkDeletePerishedBatches(IEnumerable<Batch> expiredBatches)
    {
        expiredBatches.ToList().ForEach(batch => batch.IsUsed = true);
        await _context.SaveChangesAsync();
    }
}