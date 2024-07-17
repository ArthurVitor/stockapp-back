using StockApp.Models.Dtos.BatchDto;

namespace StockApp.Models.Dtos.ExitNote;

public class ListExitNoteDto
{
    public int Id { get; set; }
    public double Quantity { get; set; }
    public int ProductId { get; set; }
    public int InventoryId { get; set; }
    public ICollection<ListExitNoteBatchDto> Batches { get; set; } = [];
}
        