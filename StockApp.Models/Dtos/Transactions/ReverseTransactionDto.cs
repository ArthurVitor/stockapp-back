using System.ComponentModel.DataAnnotations;

namespace StockApp.Models.Dtos.Transactions;

public class ReverseTransactionDto
{
    [Required]
    public int OverrideId { get; set; }
}
