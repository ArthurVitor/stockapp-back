using Microsoft.AspNetCore.Mvc;
using StockApp.Models.Dtos.ExitNote;

namespace StockApp.API.Services.Interfaces;

public interface IExitNoteService
{
    Task<IActionResult> CreateExitNoteAsync(CreateExitNoteDto exitNote);

    Task<IActionResult> GetAllExitNotesAsync(int skip, int take, string orderBy, bool isAscending);

    Task<IActionResult> GetExitNoteByIdAsync(int id);
}
