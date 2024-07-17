using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using StockApp.API.Repositories.Interfaces;
using StockApp.API.Services.Interfaces;
using StockApp.Models.Dtos.AdminRoute;
using StockApp.Models.Dtos.BatchDto;
using StockApp.Models.Dtos.ExitNote;
using StockApp.Models.Dtos.Note;
using StockApp.Models.Dtos.Pagination;
using StockApp.Models.Dtos.Transactions;
using StockApp.Models.Enum;

namespace StockApp.API.Services;

public class AdminRouteService : IAdminRouteService
{
    private readonly IExitNoteService _exitNoteService;
    private readonly IReverseExitTransaction _reverseExitTransaction;
    private readonly IReverseEntryTransacion _reverseEntryTransacion;
    private readonly IEntryNoteService _entryNoteService;
    private readonly ITransactionsService _transactionsService;
    private readonly ITransactionsRepository _transactionsRepository;
    private readonly IMapper _mapper;
    private readonly IAdminRouteRepository _adminRouteRepository;

    public AdminRouteService(IExitNoteService exitNoteService, IReverseExitTransaction reverseExitTransaction, IReverseEntryTransacion reverseEntryTransacion, IEntryNoteService entryNoteService, ITransactionsService transactionsService, ITransactionsRepository transactionsRepository, IMapper mapper, IAdminRouteRepository adminRouteRepository)
    {
        _exitNoteService = exitNoteService;
        _entryNoteService = entryNoteService;
        _mapper = mapper;
        _reverseExitTransaction = reverseExitTransaction;
        _reverseEntryTransacion = reverseEntryTransacion;
        _transactionsService = transactionsService;
        _transactionsRepository = transactionsRepository;
        _adminRouteRepository = adminRouteRepository;
    }

    public async Task<IActionResult> ReallocateBatch(CreateReallocateProductDto reallocateProductDto)
    {
        if (reallocateProductDto.SenderInventoryId == reallocateProductDto.RecipientInventoryId)
            return new BadRequestObjectResult("It is not possible to reallocate a batch to its originating inventory.");

        var createExitNoteDto = _mapper.Map<CreateExitNoteDto>(reallocateProductDto);
        var responseExitNote = await _exitNoteService.CreateExitNoteAsync(createExitNoteDto);
        if (responseExitNote is not OkObjectResult)
            return responseExitNote;


        var createEntryNoteDto = _mapper.Map<CreateEntryNoteDto>(reallocateProductDto);
        var responseEntryNote = await _entryNoteService.CreateEntryNoteAsync(createEntryNoteDto);
        if (responseEntryNote is not OkObjectResult)
            return responseEntryNote;

        return new OkResult();
    }

    public async Task<IActionResult> GetAllExpiredBatches(int skip, int take, string orderBy, bool isAscending)
    {
        var (totalRecords, expiredBatches) = await _adminRouteRepository.GetAllExpiredBatches(skip, take, orderBy, isAscending);
        var expiredBatchesDto = _mapper.Map<IEnumerable<ListBatchDto>>(expiredBatches);
        return new OkObjectResult(new PagedResultDto<ListBatchDto>(totalRecords, expiredBatchesDto));
    }

    public async Task<IActionResult> DeleteAllPerishedBatches()
    {
        var expiredBatches = await _adminRouteRepository.GetAllExpiredBatches();

        await _adminRouteRepository.BulkDeletePerishedBatches(expiredBatches);

        return new NoContentResult();
    }

    public async Task<IActionResult> ReverseTransactionAsync(ReverseTransactionDto reverseTransactionDto)
    {
        var transactionResult = await _transactionsService.GetTransactionByOverrideId(reverseTransactionDto.OverrideId);
        var transactionValue = transactionResult.Value;

        if (transactionValue is null)
            return transactionResult.Result;

        if (transactionValue.Reversed)
            return new BadRequestObjectResult($"This transaction has already been reversed.");

        await _transactionsRepository.UpdateReversedTransactionAsync(transactionValue);

        if (transactionValue.TransactionType == TransactionTypeEnum.Entry)
            return await _reverseEntryTransacion.ReverseEntryNoteAsync(transactionValue.NoteId);
        else
            return await _reverseExitTransaction.ReverseExitNoteAsync(transactionValue.NoteId);
    }
}