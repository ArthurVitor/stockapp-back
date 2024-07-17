using StockApp.Models.Dtos.BatchDto;
using StockApp.Models.Dtos.Note;

namespace StockApp.Models.Dtos.EntryNoteBatchDto;

public class ListEntryNoteBatchDto
{
    public ListEntryNoteDto EntryNote { get; set; }
    public ListBatchDto Batch { get; set; }
}
