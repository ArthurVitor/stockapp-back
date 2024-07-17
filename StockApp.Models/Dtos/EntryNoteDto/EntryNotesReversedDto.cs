using StockApp.Models.Dtos.BatchDto;
using StockApp.Models.Dtos.Note;

namespace StockApp.Models.Dtos.EntryNoteDto;

public class EntryNotesReversedDto
{
    public ICollection<ListEntryNoteDto> EntryNotes{ get; set; }

    public ICollection<ListBatchDto> Batches { get; set; }
}
    