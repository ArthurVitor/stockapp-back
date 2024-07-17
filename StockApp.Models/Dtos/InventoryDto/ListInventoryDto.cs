using StockApp.Models.Dtos.BatchDto;

namespace StockApp.Models.Dtos.InventoryDto;

public class ListInventoryDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public List<ListInventoryBatchDto> Batches { get; set; } = [];
}
