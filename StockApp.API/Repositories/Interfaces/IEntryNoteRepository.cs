using Microsoft.EntityFrameworkCore.Storage;
using StockApp.Models.Models;

namespace StockApp.API.Repositories.Interfaces;

public interface IEntryNoteRepository
{
    Task<(int totalRecords, IEnumerable<EntryNote> records)> GetAllEntryNotesAsync(int skip, int take, string orderBy, bool isAscending);

    Task<EntryNote> GetEntryNoteByIdAsync(int id);

    Task CreateEntryNoteAsync(EntryNote entryNote);

    Task<IDbContextTransaction> BeginTransactionAsync();
}
