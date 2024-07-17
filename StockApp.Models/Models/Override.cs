using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace StockApp.Models.Models;

public class Override
{
    public Override()
    {
        GenerationTime = DateTime.Now;
    }

    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    public DateTime GenerationTime { get; private set; }

    [Required]
    public string Description { get; set; } = string.Empty;

    [Required]
    public int TransactionsId { get; set; }

    public virtual Transactions Transactions { get; set; }
}
