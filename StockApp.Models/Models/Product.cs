using StockApp.Models.Enum;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StockApp.Models.Models;

public class Product
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required(ErrorMessage = "Name is required")]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Category is required")]
    public CategoryEnum Category { get; set; }

    [Required(ErrorMessage = "Perishable is required")]
    public bool Perishable { get; set; } = false;
  
    public ICollection<ProductSubCategory> ProductSubCategories { get; set; } = [];
  
    public virtual ICollection<EntryNote> EntryNotes { get; set; } = [];

    public virtual ICollection<Batch> Batches { get; set; } = [];
}