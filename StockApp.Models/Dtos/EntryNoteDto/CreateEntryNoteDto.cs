using System.ComponentModel.DataAnnotations;

namespace StockApp.Models.Dtos.Note;

public class CreateEntryNoteDto
{
    public DateTime? ExpiryDate { get; set; }

    [Required]
    [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = true)]
    [Range(0, double.MaxValue, ErrorMessage = "Price must be positive.")]
    public double Price { get; set; }

    [Required]
    [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = true)]
    [Range(0, double.MaxValue, ErrorMessage = "Quantity must be positive.")]
    public double Quantity { get; set; }

    [Required]
    public int ProductId { get; set; }

    [Required]
    public int InventoryId { get; set; }
}
