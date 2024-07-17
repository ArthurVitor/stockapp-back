using StockApp.Models.Dtos.ExitNote;
using StockApp.Models.Dtos.Note;
using StockApp.Models.Enum;

namespace StockApp.Models.Dtos.Transactions;

public class ListTransactionsDto
{
    public int Id { get; set; }
    
    public TransactionTypeEnum TransactionType { get; set; }

    public string Description { get; set; } = string.Empty;
    
    public double Quantity { get; set; }
    
    public int InventoryId { get; set; }
    
    public int NoteId { get; set; }
    
    public ListEntryNoteDto? EntryNoteDetails { get; set; }
    
    public ListExitNoteDto? ExitNoteDetails { get; set; }
}