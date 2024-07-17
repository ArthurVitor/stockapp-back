using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using StockApp.API.Repositories.Interfaces;
using StockApp.API.Services.Interfaces;
using StockApp.Models.Dtos.ExitNote;
using StockApp.Models.Models;

namespace StockApp.API.Services;

public class ReverseEntryTransaction : IReverseEntryTransacion
{
    private readonly IEntryNoteRepository _entryNoteRepository;
    private readonly IExitNoteService _exitNoteService;
    private readonly IMapper _mapper;

    public ReverseEntryTransaction(IEntryNoteRepository entryNoteRepository, IExitNoteService exitNoteService, IMapper mapper)
    {
        _entryNoteRepository = entryNoteRepository;
        _exitNoteService = exitNoteService;
        _mapper = mapper;
    }

    public async Task<IActionResult> ReverseEntryNoteAsync(int entryNoteId)
    {
        var entryNoteFromTransaction = await _entryNoteRepository.GetEntryNoteByIdAsync(entryNoteId);
        if (entryNoteFromTransaction is null)
            return new BadRequestObjectResult($"Entry note used by the transaction was not found.");
        return await CreateExitNoteFromEntryNoteAsync(entryNoteFromTransaction); 
    }

    public async Task<IActionResult> CreateExitNoteFromEntryNoteAsync(EntryNote note)
    {
        var createExitNoteDto = _mapper.Map<CreateExitNoteDto>(note);
        var result = await _exitNoteService.CreateExitNoteAsync(createExitNoteDto);
        return result;
    }
}
