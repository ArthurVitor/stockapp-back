using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using StockApp.API.Context;
using StockApp.API.Extensions;
using StockApp.API.Repositories.Interfaces;
using StockApp.Models.Models;

namespace StockApp.API.Repositories;

public class EntryNoteRepository : IEntryNoteRepository
{
    public AppDbContext _context;

    public EntryNoteRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task CreateEntryNoteAsync(EntryNote entrynote)
    {
        entrynote.TotalValue = entrynote.Quantity * entrynote.Price;
        await _context.AddAsync(entrynote);
        await _context.SaveChangesAsync();
    }

    public async Task<(int totalRecords, IEnumerable<EntryNote> records)> GetAllEntryNotesAsync(int skip, int take, string orderBy, bool isAscending)
    {
        var totalRecords = await _context.EntryNotes.CountAsync();
        var entryNotes = await _context.EntryNotes
            .AsQueryable()
            .OrderByField(orderBy, isAscending)
            .Skip(skip)
            .Take(take)
            .ToListAsync();
        return (totalRecords, entryNotes);
    }

    public async Task<EntryNote> GetEntryNoteByIdAsync(int id)
    {
        return await _context.EntryNotes.FindAsync(id);
    }

    public async Task<IDbContextTransaction> BeginTransactionAsync()
    {
        return await _context.Database.BeginTransactionAsync();
    }
}
