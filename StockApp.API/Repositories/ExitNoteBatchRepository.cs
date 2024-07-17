using StockApp.API.Context;
using StockApp.API.Repositories.Interfaces;
using StockApp.Models.Models;

namespace StockApp.API.Repositories;

public class ExitNoteBatchRepository : IExitNoteBatchRepository
{
    private readonly AppDbContext _context;

    public ExitNoteBatchRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task CreateExitNoteBatchesAsync(IEnumerable<ExitNoteBatch> exitNotesBatches)
    {
        await _context.ExitNoteBatches.AddRangeAsync(exitNotesBatches);
        await _context.SaveChangesAsync();
    }
}
