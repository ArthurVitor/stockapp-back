using Microsoft.AspNetCore.Mvc;
using StockApp.Models.Models;

namespace StockApp.API.Repositories.Interfaces;

public interface IParametersRepository
{
    Task<Parameters> CreateProductParameterAsync(Parameters parameters);

    Task<(int totalRecords, IEnumerable<Parameters> records)> GetAllParametersAsync(int skip, int take, string orderBy, bool isAscending);

    Task<Parameters> GetParameterByIdAsync(int id);

    Task<Parameters> GetParameterByInventoryIdAndProductIdAsync(int inventoryId, int productId);
}