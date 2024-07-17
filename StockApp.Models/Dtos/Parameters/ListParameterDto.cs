namespace StockApp.Models.Dtos.Parameters;

public class ListParameterDto
{
    public int Id { get; set; }

    public int ProductId { get; set; }

    public double MaximumAmount { get; set; }

    public double MinimumAmount { get; set; }

    public int InventoryId { get; set; }
}