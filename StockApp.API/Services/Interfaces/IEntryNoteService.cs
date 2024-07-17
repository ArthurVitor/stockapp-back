using Microsoft.AspNetCore.Mvc;
using StockApp.Models.Dtos.Note;

namespace StockApp.API.Services.Interfaces;

public interface IEntryNoteService
{
    Task<IActionResult> CreateEntryNoteAsync(CreateEntryNoteDto entryNoteDto);

    Task<IActionResult> GetAllEntryNotesAsync(int skip, int take, string orderBy, bool isAscending);

    Task<IActionResult> GetEntryNoteByIdAsync(int id);
}
