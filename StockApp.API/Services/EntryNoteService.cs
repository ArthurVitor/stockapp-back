using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using StockApp.API.Repositories.Interfaces;
using StockApp.API.Services.Interfaces;
using StockApp.Models.Dtos.BatchDto;
using StockApp.Models.Dtos.EntryNoteBatchDto;
using StockApp.Models.Dtos.Note;
using StockApp.Models.Dtos.Pagination;
using StockApp.Models.Enum;
using StockApp.Models.Models;
namespace StockApp.API.Services;

public class EntryNoteService : IEntryNoteService
{
    private readonly IEntryNoteRepository _entryNoteRepository;
    private readonly IMapper _mapper;
    private readonly IProductRepository _productRepository;
    private readonly IBatchService _batchService;
    private readonly IInventoryRepository _inventoryRepository;
    private readonly ITransactionsService _transactionsService;
    private readonly IInventoryService _inventoryService;

    public EntryNoteService(IEntryNoteRepository entryNoteRepository, IMapper mapper, IProductRepository productRepository, IBatchService batchService, IInventoryRepository inventoryRepository, IInventoryService inventoryService, ITransactionsService transactionsService)
    {
        _entryNoteRepository = entryNoteRepository; 
        _mapper = mapper;
        _productRepository = productRepository;
        _batchService = batchService;
        _inventoryRepository = inventoryRepository;
        _transactionsService = transactionsService;
        _inventoryService = inventoryService;
    }

    public async Task<IActionResult> CreateEntryNoteAsync(CreateEntryNoteDto entryNoteDto)
    {
        using var transaction = await _entryNoteRepository.BeginTransactionAsync();
        try
        {
            var validationResult = await ValidateEntryNoteDtoAsync(entryNoteDto);

            if (validationResult is not OkResult)
                return validationResult;

            var entryNote = _mapper.Map<EntryNote>(entryNoteDto);

            await _entryNoteRepository.CreateEntryNoteAsync(entryNote);

            var createdBatchDto = await HandleBatchCreateAsync(entryNote);
            var createdEntryNoteDto = _mapper.Map<ListEntryNoteDto>(entryNote);

            await transaction.CommitAsync();

            await _transactionsService.CreateTransaction(entryNote, TransactionTypeEnum.Entry);
            await _inventoryService.ValidateInventoryParametersAsync(entryNote);

            return new OkObjectResult(new ListEntryNoteBatchDto{ EntryNote = createdEntryNoteDto, Batch = createdBatchDto});
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<IActionResult> GetAllEntryNotesAsync(int skip, int take, string orderBy, bool isAscending)
    {
        var (totalRecords,entryNotes) = await _entryNoteRepository.GetAllEntryNotesAsync(skip, take, orderBy, isAscending);
        var entryNotesDto = _mapper.Map<IEnumerable<ListEntryNoteDto>>(entryNotes);
        return new OkObjectResult(new PagedResultDto<ListEntryNoteDto>(totalRecords, entryNotesDto));
    }

    public async Task<IActionResult> GetEntryNoteByIdAsync(int id)
    {
        var entryNote = await _entryNoteRepository.GetEntryNoteByIdAsync(id);
        if (entryNote is null)
            return new NotFoundResult();

        var entryNoteDto = _mapper.Map<ListEntryNoteDto>(entryNote);
        return new OkObjectResult(entryNoteDto);
    }

    private async Task<IActionResult> ValidateEntryNoteDtoAsync(CreateEntryNoteDto entryNoteDto)
    {
        var product = await _productRepository.GetByIdAsync(entryNoteDto.ProductId);
        if (product == null)
            return new BadRequestObjectResult("Product not found.");
        var inventory = await _inventoryRepository.GetInventoryByIdAsync(entryNoteDto.InventoryId);
        if (inventory == null)
            return new BadRequestObjectResult("Inventory not found.");

        return new OkResult();
    }

    private async Task<ListBatchDto> HandleBatchCreateAsync(EntryNote entrynote)
    {  
        var batch = await _batchService.CreateBatchAsync(entrynote);
        var inventory = await _inventoryRepository.GetInventoryByIdAsync(entrynote.InventoryId); 

        inventory.Batches.Add(batch);
        batch.InventoryId = entrynote.InventoryId;

        return _mapper.Map<ListBatchDto>(batch);
    }
}
