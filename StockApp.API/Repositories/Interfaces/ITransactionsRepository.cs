using StockApp.Models.Enum;
using StockApp.Models.Models;

namespace StockApp.API.Repositories.Interfaces;

public interface ITransactionsRepository
{
    Task CreateTransaction(Transactions transaction);

    Task<(int totalRecords, IEnumerable<Transactions> records)> GetAllTransactions(int skip, int take, string orderBy, bool isAscending);

    Task<IEnumerable<Transactions>> GetAllTransactionsByType(TransactionTypeEnum typeEnum);

    Transactions? GetTransactionById(int id);

    Transactions? GetTransactionByNoteIdAndNoteType(int noteId, TransactionTypeEnum typeEnum);

    Task UpdateReversedTransactionAsync(Transactions transaction);

}