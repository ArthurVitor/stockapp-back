using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using StockApp.API.Repositories.Interfaces;
using StockApp.API.Services.Interfaces;
using StockApp.Models.Dtos.ExitNote;
using StockApp.Models.Dtos.Pagination;
using StockApp.Models.Enum;
using StockApp.Models.Models;

namespace StockApp.API.Services;

public class ExitNoteService : IExitNoteService
{
    private readonly IExitNoteRepository _exitNoteRepository;
    private readonly IProductRepository _productRepository;
    private readonly IBatchService _batchService;
    private readonly IInventoryService _inventoryService;
    private readonly IExitNoteBatchService _exitNoteBatchService;
    private readonly ITransactionsService _transactionsService;
    private readonly IMapper _mapper;

    public ExitNoteService(IExitNoteRepository exitNoteRepository, IProductRepository productRepository, IMapper mapper, IExitNoteBatchService exitNoteBatchService, IBatchService batchService, IInventoryService inventoryService, ITransactionsService transactionsService)
    {
        _exitNoteRepository = exitNoteRepository;
        _productRepository = productRepository;
        _mapper = mapper;
        _exitNoteBatchService = exitNoteBatchService;
        _batchService = batchService;
        _inventoryService = inventoryService;
        _transactionsService = transactionsService;
    }

    public async Task<IActionResult> CreateExitNoteAsync(CreateExitNoteDto createExitNoteDto)
    {
        using var transaction = await _exitNoteRepository.BeginTransactionAsync();
        try
        {
            var exitNote = _mapper.Map<ExitNote>(createExitNoteDto);

            var validationEntityResult = await ValidateExitNoteEntitiesDataAsync(exitNote);
            if (validationEntityResult is not OkResult)
                return validationEntityResult;

            var (availableBatchesWithThisProduct, batchValidationResult) = await _inventoryService.ValidateAvailableBatchesAsync(exitNote.InventoryId, exitNote.ProductId, exitNote.Quantity);
            if (batchValidationResult is not OkResult)
                return batchValidationResult;

            var usedBatches = await _batchService.DeductQuantityFromBatchesAsync(availableBatchesWithThisProduct, exitNote.Quantity);

            await _exitNoteRepository.CreateExitNoteAsync(exitNote);

            var exitNoteBatchResult = await _exitNoteBatchService.CreateExitNoteBatchAsync(exitNote, usedBatches);
            if (exitNoteBatchResult is not OkObjectResult)
                return exitNoteBatchResult;

            await transaction.CommitAsync();

            await _transactionsService.CreateTransaction(exitNote, TransactionTypeEnum.Exit);
            await _inventoryService.ValidateInventoryParametersAsync(exitNote);
            
            var listExitNoteDto = _mapper.Map<ListExitNoteDto>(exitNote);
            return new OkObjectResult(listExitNoteDto);
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<IActionResult> GetAllExitNotesAsync(int skip, int take, string orderBy, bool isAscending)
    {
        var (totalRecords,exitNotes) = await _exitNoteRepository.GetAllExitNotesAsync(skip, take, orderBy, isAscending);
        var exitNoteDtos = _mapper.Map<IEnumerable<ListExitNoteDto>>(exitNotes);
        return new OkObjectResult(new PagedResultDto<ListExitNoteDto>(totalRecords, exitNoteDtos));
    }

    public async Task<IActionResult> GetExitNoteByIdAsync(int id)
    {
        var exitNote = await _exitNoteRepository.GetExitNoteByIdAsync(id);
        if (exitNote is null)
            return new NotFoundResult();

        var exitNoteDto = _mapper.Map<ListExitNoteDto>(exitNote);
        return new OkObjectResult(exitNoteDto);
    }

    public async Task<IActionResult> ValidateExitNoteEntitiesDataAsync(ExitNote exitNote)
    {
        var product = await _productRepository.GetByIdAsync(exitNote.ProductId);
        if (product is null)
            return new BadRequestObjectResult("Product not found.");
        var inventory = await _inventoryService.GetInventoryByIdAsync(exitNote.InventoryId);
        if (inventory is not OkObjectResult)
            return new BadRequestObjectResult("Inventory not found.");

        return new OkResult();
    }
}