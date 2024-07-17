using Microsoft.AspNetCore.Mvc;
using StockApp.API.Services;
using StockApp.API.Services.Interfaces;
using StockApp.Models.Dtos.ExitNote;

namespace StockApp.API.Controllers;


[ApiController]
[Route("api/[controller]")]
public class ExitNoteController : ControllerBase
{
    private readonly IExitNoteService _exitNoteService;

    public ExitNoteController(IExitNoteService exitNoteService)
    {
        _exitNoteService = exitNoteService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateExitNoteAsync([FromBody] CreateExitNoteDto createExitNoteDto)
    {
        return await _exitNoteService.CreateExitNoteAsync(createExitNoteDto);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllExitNotesAsync([FromQuery] int skip = 0, [FromQuery] int take = 10, [FromQuery] string orderBy = "id", [FromQuery] bool isAscending = true)
    {
        return await _exitNoteService.GetAllExitNotesAsync(skip, take, orderBy, isAscending);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetExitNoteByIdAsync(int id)
    {
        return await _exitNoteService.GetExitNoteByIdAsync(id);
    }
}
