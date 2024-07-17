using Microsoft.EntityFrameworkCore.Storage;
using StockApp.Models.Models;

namespace StockApp.API.Repositories.Interfaces;

public interface IExitNoteRepository
{
    Task CreateExitNoteAsync(ExitNote exitNote);
    Task<IDbContextTransaction> BeginTransactionAsync();
    Task<(int totalRecords, IEnumerable<ExitNote> records)> GetAllExitNotesAsync(int skip, int take, string orderBy, bool isAscending);
    Task<ExitNote> GetExitNoteByIdAsync(int id);
}
