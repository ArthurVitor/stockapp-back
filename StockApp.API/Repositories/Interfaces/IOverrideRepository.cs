using StockApp.Models.Models;

namespace StockApp.API.Repositories.Interfaces;

public interface IOverrideRepository
{
    Task CreateOverrideAsync(Override overrideObject);

    Task<(int totalRecords, IEnumerable<Override> records)> GetAllOverridesAsync(int skip, int take, string orderBy, bool isAscending);

    Task<Override> GetOverrideByIdAsync(int id);
}
