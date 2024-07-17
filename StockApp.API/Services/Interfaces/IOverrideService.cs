using Microsoft.AspNetCore.Mvc;
using StockApp.Models.Enum;
using StockApp.Models.Models;

namespace StockApp.API.Services.Interfaces;

public interface IOverrideService
{
    Task<Override> CreateOverrideAsync(NoteBase note, string surpassed, double quantitySurpassed);

    Task<IActionResult> GetAllOverridesAsync(int skip, int take, string orderBy, bool isAscending);

    Task<IActionResult> GetOverrideByIdAsync(int id);

    Task ValidateOverrideAsync(NoteBase note, double totalAvailableQuantity, Parameters parameters);
}
