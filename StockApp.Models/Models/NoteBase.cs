using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StockApp.Models.Models;

public class NoteBase
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    
    [Required]
    [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = true)]
    [Range(0, double.MaxValue, ErrorMessage = "Quantity must be positive.")]
    public double Quantity { get; set; }
    
    public virtual Product Product { get; set; }
    
    [Required]
    public int InventoryId { get; set; }
}