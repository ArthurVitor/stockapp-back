using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using StockApp.Models.Enum;

namespace StockApp.Models.Models;

public class Transactions
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    
    public TransactionTypeEnum TransactionType { get; set; }

    public string Description { get; set; } = string.Empty;
    
    public double Quantity { get; set; }

    [Required]
    public bool Reversed { get; set; } = false;

    public int InventoryId { get; set; }
    
    [ForeignKey("InventoryId")]
    public Inventory Inventory { get; set; }
    
    public int NoteId { get; set; }
}