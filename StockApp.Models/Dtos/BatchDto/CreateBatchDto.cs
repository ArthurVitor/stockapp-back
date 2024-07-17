using System.ComponentModel.DataAnnotations;

namespace StockApp.Models.Dtos.BatchDto;

public class CreateBatchDto
{
    public DateTime? ExpiryDate { get; set; }

    [Required]
    [Range(0, double.MaxValue, ErrorMessage = "Price must be positive.")]
    public double Price { get; set; }

    [Required]
    [Range(0, double.MaxValue, ErrorMessage = "Quantity must be positive.")]
    public double Quantity { get; set; }

    [Required]
    [Range(0, double.MaxValue, ErrorMessage = "TotalValue must be positive.")]
    public double TotalValue { get; set; }

    [Required]
    public int ProductId { get; set; }

    [Required]
    public int EntryNoteId { get; set; }

    [Required]
    public int InventoryId { get; set; }
}
