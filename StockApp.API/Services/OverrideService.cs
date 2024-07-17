using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using StockApp.API.Repositories.Interfaces;
using StockApp.API.Services.Interfaces;
using StockApp.Models.Dtos.OverrideDto;
using StockApp.Models.Dtos.Pagination;
using StockApp.Models.Enum;
using StockApp.Models.Models;

namespace StockApp.API.Services;

public class OverrideService : IOverrideService
{
    private readonly IOverrideRepository _overrideRepository;
    private readonly ITransactionsRepository _transactionsRepository;
    private readonly IMapper _mapper;

    public OverrideService(IOverrideRepository overrideRepository, ITransactionsRepository transactionsRepository, IMapper mapper)
    {
        _overrideRepository = overrideRepository;
        _transactionsRepository = transactionsRepository;
        _mapper = mapper;
    }

    public async Task<Override> CreateOverrideAsync(NoteBase note, string surpassed, double quantitySurpassed)
    {
        var type = note is EntryNote ? TransactionTypeEnum.Entry : TransactionTypeEnum.Exit;
        Transactions transaction = _transactionsRepository.GetTransactionByNoteIdAndNoteType(note.Id, type)!;

        string description = $"The inventory with id {transaction.InventoryId} is {quantitySurpassed} units {surpassed} the allowed amount for the product with id {note.Product.Id}";

        Override overrideObject = new()
        {
            TransactionsId = transaction.Id,
            Description = description
        };

        await _overrideRepository.CreateOverrideAsync(overrideObject);
        return overrideObject;
    }

    public async Task<IActionResult> GetAllOverridesAsync(int skip, int take, string orderBy, bool isAscending)
    {
        var (totalRecords, overrides) = await _overrideRepository.GetAllOverridesAsync(skip, take, orderBy, isAscending);
        var overridesDto = _mapper.Map<IEnumerable<ListOverrideDto>>(overrides);
        return new OkObjectResult(new PagedResultDto<ListOverrideDto>(totalRecords, overridesDto));
    }

    public async Task<IActionResult> GetOverrideByIdAsync(int id)
    {
        var overrideObject = await _overrideRepository.GetOverrideByIdAsync(id);
        if (overrideObject is null)
            return new NotFoundResult();

        var overrideDto = _mapper.Map<ListOverrideDto>(overrideObject);
        return new OkObjectResult(overrideDto);
    }

    public async Task ValidateOverrideAsync(NoteBase note, double totalAvailableQuantity, Parameters parameters)
    {
        if (note is EntryNote)
            await ValidateOverrideEntryNoteAsync(note, totalAvailableQuantity, parameters);
        else
            await ValidateOverrideExitNoteAsync(note, totalAvailableQuantity, parameters);
    }

    private async Task ValidateOverrideEntryNoteAsync(NoteBase note, double totalAvailableQuantity, Parameters parameters)
    {
        if (totalAvailableQuantity > parameters.MaximumAmount)
            await CreateOverrideAsync(note, "above", totalAvailableQuantity - parameters.MaximumAmount);
    }

    private async Task ValidateOverrideExitNoteAsync(NoteBase note, double totalAvailableQuantity, Parameters parameters)
    {
        if (totalAvailableQuantity < parameters.MinimumAmount)
            await CreateOverrideAsync(note, "below", parameters.MinimumAmount - totalAvailableQuantity);
    }
}
