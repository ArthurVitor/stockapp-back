using Microsoft.AspNetCore.Mvc;
using StockApp.Models.Enum;
using StockApp.Models.Models;

namespace StockApp.API.Services.Interfaces;

public interface IReverseEntryTransacion
{
    public Task<IActionResult> ReverseEntryNoteAsync(int entryNoteId);

    public Task<IActionResult> CreateExitNoteFromEntryNoteAsync(EntryNote note);
}
