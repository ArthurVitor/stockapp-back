using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace StockApp.Models.Models;

public class ExitNote : NoteBase
{
    public ExitNote()
    {
        NoteGenerationTime = DateTime.Now;
    }

    [Required]
    public DateTime NoteGenerationTime { get; private set; }

    [Required]
    public int ProductId { get; set; }

    public virtual ICollection<ExitNoteBatch> ExitNoteBatches { get; set; } = [];
    public virtual Inventory Inventory { get; set; }
}
