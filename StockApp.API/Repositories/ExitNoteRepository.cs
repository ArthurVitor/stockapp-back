using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using StockApp.API.Context;
using StockApp.API.Extensions;
using StockApp.API.Repositories.Interfaces;
using StockApp.Models.Models;

namespace StockApp.API.Repositories;

public class ExitNoteRepository : IExitNoteRepository
{
    public AppDbContext _context;

    public ExitNoteRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task CreateExitNoteAsync(ExitNote exitNote)
    {
        await _context.ExitNotes.AddAsync(exitNote);
        await _context.SaveChangesAsync();
    }

    public async Task<(int totalRecords, IEnumerable<ExitNote> records)> GetAllExitNotesAsync(int skip, int take, string orderBy, bool isAscending)
    {
        var totalRecords = await _context.ExitNotes.CountAsync();
        var exitNotes = await _context.ExitNotes
            .AsQueryable().OrderByField(orderBy, isAscending)
            .Include(en => en.ExitNoteBatches)
            .ThenInclude(enb => enb.Batch)
            .Skip(skip)
            .Take(take)
            .ToListAsync();
        return (totalRecords, exitNotes);
    }

    public async Task<ExitNote> GetExitNoteByIdAsync(int id)
    {
        return await _context.ExitNotes
                .Include(en => en.ExitNoteBatches)
                .ThenInclude(enb => enb.Batch)
                .FirstOrDefaultAsync(en => en.Id == id);
    }

    public async Task<IDbContextTransaction> BeginTransactionAsync()
    {
        return await _context.Database.BeginTransactionAsync();
    }
}
