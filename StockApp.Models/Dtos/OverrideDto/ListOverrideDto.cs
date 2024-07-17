namespace StockApp.Models.Dtos.OverrideDto;

public class ListOverrideDto
{
    public int Id { get; set; }
    public DateTime GenerationTime { get; private set; }
    public string Description { get; set; } = string.Empty;
    public int TransactionsId { get; set; }
}
