using Microsoft.EntityFrameworkCore;
using StockApp.API.Context;
using StockApp.API.Extensions;
using StockApp.API.Repositories.Interfaces;
using StockApp.Models.Enum;
using StockApp.Models.Models;

namespace StockApp.API.Repositories;

public class TransactionsRepository : ITransactionsRepository
{
    private readonly AppDbContext _context;

    public TransactionsRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task CreateTransaction(Transactions transaction)
    {
        await _context.AddAsync(transaction);
        await _context.SaveChangesAsync();
    }

    public async Task<(int totalRecords, IEnumerable<Transactions> records)> GetAllTransactions(int skip, int take, string orderBy, bool isAscending)
    {
        var totalRecords = await _context.Transactions.CountAsync();
        var transactions = await _context.Transactions
            .AsQueryable()
            .OrderByField(orderBy, isAscending)
            .Skip(skip)
            .Take(take)
            .ToListAsync();
        
        return (totalRecords, transactions);
    }

    public async Task<IEnumerable<Transactions>> GetAllTransactionsByType(TransactionTypeEnum typeEnum)
    {
        return await _context.Transactions.Where(t => t.TransactionType == typeEnum).ToListAsync();
    }

    public Transactions? GetTransactionById(int id)
    {
        return _context.Transactions.Find(id);
    }

    public Transactions? GetTransactionByNoteIdAndNoteType(int noteId, TransactionTypeEnum typeEnum)
    {
        return _context.Transactions.FirstOrDefault(t => t.NoteId == noteId && t.TransactionType == typeEnum);
    }

    public async Task UpdateReversedTransactionAsync(Transactions transaction)
    {
        transaction.Reversed = !transaction.Reversed;
        await _context.SaveChangesAsync();
    }
}