using Microsoft.EntityFrameworkCore.Storage;
using StockApp.Models.Models;

namespace StockApp.API.Repositories.Interfaces;

public interface IAdminRouteRepository
{
    Task<(int totalRecords, IEnumerable<Batch> records)> GetAllExpiredBatches(int skip, int take, string orderBy, bool isAscending);

    Task<IEnumerable<Batch>> GetAllExpiredBatches(); 

    Task BulkDeletePerishedBatches(IEnumerable<Batch> expiredBatches);
}