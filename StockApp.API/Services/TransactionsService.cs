using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using StockApp.API.Repositories.Interfaces;
using StockApp.API.Services.Interfaces;
using StockApp.Models.Dtos.ExitNote;
using StockApp.Models.Dtos.Note;
using StockApp.Models.Dtos.Pagination;
using StockApp.Models.Dtos.Transactions;
using StockApp.Models.Enum;
using StockApp.Models.Models;

namespace StockApp.API.Services;

public class TransactionsService : ITransactionsService
{
    private readonly ITransactionsRepository _transactionsRepository;
    private readonly IOverrideRepository _overrideRepository;
    private readonly IMapper _mapper;
    private readonly IEntryNoteRepository _entryNoteRepository;
    private readonly IExitNoteRepository _exitNoteRepository;

    public TransactionsService(ITransactionsRepository transactionsRepository, IMapper mapper, IEntryNoteRepository entryNoteRepository, IExitNoteRepository exitNoteRepository, IOverrideRepository overrideRepository)
    {
        _transactionsRepository = transactionsRepository;
        _mapper = mapper;
        _entryNoteRepository = entryNoteRepository;
        _exitNoteRepository = exitNoteRepository;
        _overrideRepository = overrideRepository;
    }

    public async Task CreateTransaction(NoteBase note, TransactionTypeEnum transactionType)
    {
        string action = transactionType == TransactionTypeEnum.Entry ? "added" : "removed";
        var description = $"{transactionType} Note: {note.Id} {action} {note.Quantity} to Inventory: {note.InventoryId}.";
        var transaction = new Transactions()
        {
            TransactionType = transactionType,
            Description = description,
            Quantity = note.Quantity,
            InventoryId = note.InventoryId,
            NoteId = note.Id
        };

        await _transactionsRepository.CreateTransaction(transaction);
    }

    public async Task<IActionResult> GetAllTransaction(int skip, int take, string orderBy, bool isAscending)
    {
        var (totalRecords, transactionObjects) = await _transactionsRepository.GetAllTransactions(skip, take, orderBy, isAscending);
        List<ListTransactionsDto> transactionDtos = [];

        foreach (var transaction in transactionObjects)
        {
            var transactionDto = _mapper.Map<Transactions, ListTransactionsDto>(transaction);
            await SetNoteDetailsAsync(transactionDto);
            transactionDtos.Add(transactionDto);
        }

        return new OkObjectResult(new PagedResultDto<ListTransactionsDto>(totalRecords, transactionDtos));
    }

    public async Task<IActionResult> GetAllTransactionsByType(TransactionTypeEnum transactionTypeEnum)
    {
        if (transactionTypeEnum is not TransactionTypeEnum.Entry && transactionTypeEnum is not TransactionTypeEnum.Exit)
            return new BadRequestObjectResult("Invalid Transaction Type");

        var transactionObjects = await _transactionsRepository.GetAllTransactionsByType(transactionTypeEnum);
        List<ListTransactionsDto> transactionsListDtos = [];
        foreach (var transaction in transactionObjects)
        {
            var transactionDto = _mapper.Map<Transactions, ListTransactionsDto>(transaction);
            await SetNoteDetailsAsync(transactionDto);
            transactionsListDtos.Add(transactionDto);
        }

        return new OkObjectResult(transactionsListDtos);
    }

    public async Task<IActionResult> GetTransactionById(int id)
    {
        var transaction = _transactionsRepository.GetTransactionById(id);

        if (transaction is null)
            return new NotFoundObjectResult($"There's no transaction with ID {id}");

        var transactionDto = _mapper.Map<Transactions, ListTransactionsDto>(transaction);
        await SetNoteDetailsAsync(transactionDto);

        return new OkObjectResult(transactionDto);
    }

    public async Task<IActionResult> GetTransactionByNoteIdAndNoteType(int id, TransactionTypeEnum typeEnum)
    {
        if (typeEnum is not TransactionTypeEnum.Entry && typeEnum is not TransactionTypeEnum.Exit)
            return new BadRequestObjectResult("Invalid Transaction Type");

        var transaction = _transactionsRepository.GetTransactionByNoteIdAndNoteType(id, typeEnum);

        if (transaction is null)
            return new NotFoundObjectResult($"There isn't a note with Id: {id} and TransactionType: {typeEnum.ToString()}");

        var transactionDto = _mapper.Map<Transactions, ListTransactionsDto>(transaction);
        await SetNoteDetailsAsync(transactionDto);

        return new OkObjectResult(transactionDto);
    }

    public async Task<ActionResult<Transactions>> GetTransactionByOverrideId(int overrideId)
    {
        var overrideObject = await _overrideRepository.GetOverrideByIdAsync(overrideId);
        if (overrideObject is null)
            return new BadRequestObjectResult($"Overide message with id {overrideId} was not found.");

        var transaction = _transactionsRepository.GetTransactionById(overrideObject.TransactionsId);
        if (transaction is null)
            return new NotFoundObjectResult($"The transaction that originated the override message with id {overrideId} was not found.");

        return transaction;
    }

    private async Task SetNoteDetailsAsync(ListTransactionsDto transactionDto)
    {
        if (transactionDto.TransactionType == TransactionTypeEnum.Entry)
        {
            var entryNote = await _entryNoteRepository.GetEntryNoteByIdAsync(transactionDto.NoteId);
            var entryNoteDto = _mapper.Map<ListEntryNoteDto>(entryNote);
            transactionDto.EntryNoteDetails = entryNoteDto;
        }
        else
        {
            var exitNote = await _exitNoteRepository.GetExitNoteByIdAsync(transactionDto.NoteId);
            var exitNoteDto = _mapper.Map<ListExitNoteDto>(exitNote);
            transactionDto.ExitNoteDetails = exitNoteDto;
        }
    }
}