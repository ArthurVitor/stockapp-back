using System.ComponentModel.DataAnnotations;

namespace StockApp.Models.Dtos.Parameters;

public class CreateParameterDto
{
    [Required(ErrorMessage = "ProductId is required")]
    public int ProductId { get; set; }
    
    [Required(ErrorMessage = "MaximumAmount is required")]
    [Range(0, double.MaxValue, ErrorMessage = "Maximum amount must be positive.")]
    public double MaximumAmount { get; set; }
    
    [Required(ErrorMessage = "MinimumAmount is required")]
    [Range(0, double.MaxValue, ErrorMessage = "Minimum amount must be positive.")]
    public double MinimumAmount { get; set; }

    [Required(ErrorMessage = "Inventory ID is required.")]
    public int InventoryId { get; set; }
}