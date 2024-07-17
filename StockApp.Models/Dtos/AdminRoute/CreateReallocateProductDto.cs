using System.ComponentModel.DataAnnotations;

namespace StockApp.Models.Dtos.AdminRoute;

public class CreateReallocateProductDto
{
    [Required]
    public int SenderInventoryId { get; set; }
    
    [Required]
    public int RecipientInventoryId { get; set; }
    
    [Required]
    public int ProductId { get; set; }
    
    [Required]
    public double Quantity { get; set; }
    
    public DateTime? ExpiryDate { get; set; }

    [Required]
    [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = true)]
    [Range(0, double.MaxValue, ErrorMessage = "Price must be positive.")]
    public double Price { get; set; }
}