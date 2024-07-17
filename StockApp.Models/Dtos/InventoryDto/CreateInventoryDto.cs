using System.ComponentModel.DataAnnotations;

namespace StockApp.Models.Dtos.InventoryDto;

public class CreateInventoryDto
{
    [Required]
    public string Name { get; set; }
}
