using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace StockApp.Models.Models;

public class Batch
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; } 

    public DateTime? ExpiryDate { get; set; }

    [Required]
    [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = true)]
    [Range(0, double.MaxValue, ErrorMessage = "Price must be positive.")]
    public double Price { get; set; }

    [Required]
    [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = true)]
    [Range(0, double.MaxValue, ErrorMessage = "Quantity must be positive.")]
    public double Quantity { get; set; }

    public double TotalValue { get; set; }

    public bool IsUsed { get; set; } = false; 

    [Required]
    public int ProductId { get; set; }

    [Required]
    public int EntryNoteId { get; set; }

    [Required]
    public int InventoryId { get; set; }

    public bool Perished { get; set; } = false;

    public virtual ICollection<ExitNoteBatch> ExitNoteBatches { get; set; } = [];
    public virtual EntryNote EntryNote { get; set; }
    public virtual Product Product { get; set; }
    public virtual Inventory Inventory { get; set; }
}
