using System.ComponentModel.DataAnnotations;

namespace StockApp.Models.Models;

public class ExitNoteBatch
{
    [Key]
    public int BatchId { get; set; }
    [Key]
    public int ExitNoteId { get; set; }
    [Required]
    public double Quantity { get; set; }
    public virtual ExitNote ExitNote { get; set; }
    public virtual Batch Batch { get; set; }
}
