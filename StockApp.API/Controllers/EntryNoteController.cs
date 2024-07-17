using Microsoft.AspNetCore.Mvc;
using StockApp.API.Services.Interfaces;
using StockApp.Models.Dtos.Note;

namespace StockApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EntryNoteController : ControllerBase
{
    private readonly IEntryNoteService _entryNoteService;

    public EntryNoteController(IEntryNoteService entryNoteService)
    {
        _entryNoteService = entryNoteService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateEntryNoteAsync([FromBody] CreateEntryNoteDto createEntryNoteDto)
    {
        return await _entryNoteService.CreateEntryNoteAsync(createEntryNoteDto);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllEntryNotesAsync([FromQuery] int skip = 0, [FromQuery] int take = 10, [FromQuery] string orderBy = "id", [FromQuery] bool isAscending = true)
    {
        return await _entryNoteService.GetAllEntryNotesAsync(skip, take, orderBy, isAscending);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetEntryNoteByIdAsync(int id)
    {
        return await _entryNoteService.GetEntryNoteByIdAsync(id);
    }
}
