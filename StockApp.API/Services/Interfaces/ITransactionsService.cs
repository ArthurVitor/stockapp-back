using Microsoft.AspNetCore.Mvc;
using StockApp.Models.Enum;
using StockApp.Models.Models;

namespace StockApp.API.Services.Interfaces;

public interface ITransactionsService
{
    Task CreateTransaction(NoteBase note, TransactionTypeEnum transactionType);

    Task<IActionResult> GetAllTransaction(int skip, int take, string orderBy, bool isAscending);

    Task<IActionResult> GetAllTransactionsByType(TransactionTypeEnum transactionTypeEnum);

    Task<IActionResult> GetTransactionById(int id);

    Task<IActionResult> GetTransactionByNoteIdAndNoteType(int id, TransactionTypeEnum typeEnum);

    Task<ActionResult<Transactions>> GetTransactionByOverrideId(int overrideId); 
}