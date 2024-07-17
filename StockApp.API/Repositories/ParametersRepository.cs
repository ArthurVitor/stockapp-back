using Microsoft.EntityFrameworkCore;
using StockApp.API.Context;
using StockApp.API.Extensions;
using StockApp.API.Repositories.Interfaces;
using StockApp.Models.Models;

namespace StockApp.API.Repositories;

public class ParametersRepository : IParametersRepository
{
    private readonly AppDbContext _context;

    public ParametersRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Parameters> CreateProductParameterAsync(Parameters parameters)
    {
        var parameterCreated = await _context.Parameters.AddAsync(parameters);
        await _context.SaveChangesAsync();

        return parameterCreated.Entity;
    }

    public async Task<(int totalRecords, IEnumerable<Parameters> records)> GetAllParametersAsync(int skip, int take, string orderBy, bool isAscending)
    {
        var totalRecords = await _context.Parameters.CountAsync();
        var parameters = await _context.Parameters
            .AsQueryable()
            .OrderByField(orderBy, isAscending)
            .Skip(skip)
            .Take(take)
            .ToListAsync();
        return (totalRecords, parameters);
    }

    public async Task<Parameters> GetParameterByIdAsync(int id)
    {
        return await _context.Parameters.FindAsync(id);
    }

    public async Task<Parameters> GetParameterByInventoryIdAndProductIdAsync(int inventoryId, int productId)
    {
        return await _context.Parameters
            .FirstOrDefaultAsync(p => p.InventoryId == inventoryId && p.ProductId == productId);
    }
}