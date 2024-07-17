using Microsoft.AspNetCore.Mvc;
using StockApp.API.Services.Interfaces;
using StockApp.Models.Enum;

namespace StockApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TransactionsController : ControllerBase
{
    private readonly ITransactionsService _transactionsService;

    public TransactionsController(ITransactionsService transactionsService)
    {
        _transactionsService = transactionsService;
    }

    [HttpGet]
    public Task<IActionResult> GetAllTransactions([FromQuery] int skip = 0, [FromQuery] int take = 10, [FromQuery] string orderBy = "id", [FromQuery] bool isAscending = true)
    {
        return _transactionsService.GetAllTransaction(skip, take, orderBy, isAscending);
    }

    [HttpGet("{transactionType}")]
    public Task<IActionResult> GetTransaction(TransactionTypeEnum transactionType)
    {
        return _transactionsService.GetAllTransactionsByType(transactionType);
    }

    [HttpGet("id/{id}")]
    public Task<IActionResult> GetTransactionById(int id)
    {
        return _transactionsService.GetTransactionById(id);
    }

    [HttpGet("noteId/{id}/noteType/{noteType}")]
    public Task<IActionResult> GetTransactionByNoteIdAndNoteType(int id, TransactionTypeEnum noteType)
    {
        return _transactionsService.GetTransactionByNoteIdAndNoteType(id, noteType);
    }
}