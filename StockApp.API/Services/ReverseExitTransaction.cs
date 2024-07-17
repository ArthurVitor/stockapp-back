using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using StockApp.API.Repositories.Interfaces;
using StockApp.API.Services.Interfaces;
using StockApp.Models.Dtos.BatchDto;
using StockApp.Models.Dtos.EntryNoteDto;
using StockApp.Models.Dtos.Note;
using StockApp.Models.Models;

namespace StockApp.API.Services;

public class ReverseExitTransaction : IReverseExitTransaction
{
    private readonly IExitNoteRepository _exitNoteRepository;
    private readonly IEntryNoteService _entryNoteService;
    private readonly IMapper _mapper;

    public ReverseExitTransaction(IExitNoteRepository exitNoteRepository, IEntryNoteService entryNoteService, IMapper mapper)
    {
        _exitNoteRepository = exitNoteRepository;
        _entryNoteService = entryNoteService;
        _mapper = mapper;
    }

    public async Task<IActionResult> ReverseExitNoteAsync(int exitNoteId)
    {
        var exitNoteFromTransaction = await _exitNoteRepository.GetExitNoteByIdAsync(exitNoteId);
        if (exitNoteFromTransaction is null)
            return new BadRequestObjectResult("Exit note used by the transaction was not found.");
        return await CreateEntryNotesFromExitNoteAsync(exitNoteFromTransaction);
    }

    public async Task<IActionResult> CreateEntryNotesFromExitNoteAsync(ExitNote note)
    {
        var entryNotesCreated = new List<ListEntryNoteDto>();
        var batchesCreated = new List<ListBatchDto>();

        foreach (var exitNoteBatch in note.ExitNoteBatches)
        {
            var createEntryNoteDto = _mapper.Map<CreateEntryNoteDto>(exitNoteBatch);

            if (createEntryNoteDto.Quantity == 0)
                continue;

            var result = await _entryNoteService.CreateEntryNoteAsync(createEntryNoteDto);

            if (result is OkObjectResult okResult && okResult.Value is not null)
            {
                dynamic entryNoteBatchObject = okResult.Value;
                var entryNote = (ListEntryNoteDto)entryNoteBatchObject.EntryNote;
                var batch = (ListBatchDto)entryNoteBatchObject.Batch;

                if (entryNote != null && batch != null)
                {
                    batchesCreated.Add(batch);
                    entryNotesCreated.Add(entryNote);
                }
            }
            else
                return result;
        }

        var entryNotesReversedDto = _mapper.Map<EntryNotesReversedDto>((entryNotesCreated, batchesCreated));

        return new OkObjectResult(entryNotesReversedDto);
    }
}
