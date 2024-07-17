using Microsoft.AspNetCore.Mvc;
using StockApp.Models.Models;

namespace StockApp.API.Services.Interfaces;

public interface IReverseExitTransaction
{
    public Task<IActionResult> ReverseExitNoteAsync(int exitNoteId);

    public Task<IActionResult> CreateEntryNotesFromExitNoteAsync(ExitNote note);
}
