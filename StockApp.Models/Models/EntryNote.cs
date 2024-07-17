using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StockApp.Models.Models;

public class EntryNote : NoteBase
{
    public EntryNote()
    {
        NoteGenerationTime = DateTime.Now;
    }

    [Required]
    public DateTime NoteGenerationTime { get; private set; }

    public DateTime? ExpiryDate { get; set; }

    [Required]
    [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = true)]
    [Range(0, double.MaxValue, ErrorMessage = "Price must be positive.")]
    public double Price { get; set; }

    [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = true)]
    [Range(0, double.MaxValue, ErrorMessage = "TotalValue must be positive.")]
    public double TotalValue { get; set; }

    [Required]
    public int ProductId { get; set; }

    public virtual Inventory Inventory { get; set; }

    public virtual Batch Batch { get; set; }
}
