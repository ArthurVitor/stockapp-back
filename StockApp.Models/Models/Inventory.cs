using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace StockApp.Models.Models;

public class Inventory
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    public virtual ICollection<EntryNote> EntryNotes { get; set; } = [];
    public virtual ICollection<Batch> Batches { get; set; } = [];
    public virtual ICollection<ExitNote> ExitNotes { get; set; } = [];
    public virtual ICollection<Parameters> Parameters { get; set; } = []; 
}
