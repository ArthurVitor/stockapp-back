using System.ComponentModel.DataAnnotations;

namespace StockApp.Models.Dtos.ExitNote;

public class CreateExitNoteDto
{
    [Required]
    [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = true)]
    [Range(0, double.MaxValue, ErrorMessage = "Quantity must be positive.")]
    public double Quantity { get; set; }

    [Required]
    public int ProductId { get; set; }

    [Required]
    public int InventoryId { get; set; }
}
