using StockApp.Models.Models;

namespace StockApp.API.Repositories.Interfaces;

public interface IExitNoteBatchRepository
{
    Task CreateExitNoteBatchesAsync(IEnumerable<ExitNoteBatch> exitNotesBatches);
}
