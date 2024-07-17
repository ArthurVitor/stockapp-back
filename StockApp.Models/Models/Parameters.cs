using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StockApp.Models.Models;

public class Parameters
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    public int ProductId { get; set; }

    [Required]
    [Range(0, double.MaxValue, ErrorMessage = "Maximum amount must be positive.")]
    public double MaximumAmount { get; set; }

    [Required]
    [Range(0, double.MaxValue, ErrorMessage = "Minimum amount must be positive.")]
    public double MinimumAmount { get; set; }

    [Required]
    public int InventoryId { get; set; }

    public virtual Inventory Inventory { get; set; }
}