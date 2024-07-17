using Microsoft.EntityFrameworkCore;
using StockApp.API.Context;
using StockApp.API.Extensions;
using StockApp.API.Repositories.Interfaces;
using StockApp.Models.Models;

namespace StockApp.API.Repositories;

public class OverrideRepository : IOverrideRepository
{
    public AppDbContext _context;

    public OverrideRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task CreateOverrideAsync(Override overrideObject)
    {
        await _context.Overrides.AddAsync(overrideObject);
        await _context.SaveChangesAsync();
    }

    public async Task<(int totalRecords, IEnumerable<Override> records)> GetAllOverridesAsync(int skip, int take, string orderBy, bool isAscending)
    {
        var totalRecords = await _context.Overrides.CountAsync();
        var overrides =  await _context.Overrides
            .AsQueryable()
            .OrderByField(orderBy, isAscending)
            .Skip(skip)
            .Take(take)
            .ToListAsync();
        return (totalRecords, overrides);
    }

    public async Task<Override> GetOverrideByIdAsync(int id)
    {
        return await _context.Overrides
        .FirstOrDefaultAsync(i => i.Id == id);
    }
}
