using Microsoft.AspNetCore.Mvc;
using StockApp.Models.Dtos.Parameters;

namespace StockApp.API.Services.Interfaces;

public interface IParameterService
{
    Task<IActionResult> CreateParameter(CreateParameterDto parameterDto);

    Task<IActionResult> GetAllParameters(int skip, int take, string orderBy, bool isAscending);

    Task<IActionResult> GetParameterById(int id);

    Task<IActionResult> GetParameterByInventoryIdAndProductIdAsync(int inventoryId, int productId);
}