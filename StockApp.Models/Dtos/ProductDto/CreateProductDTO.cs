using System.ComponentModel.DataAnnotations;
using StockApp.Models.Enum;

namespace StockApp.Models.Dtos.ProductDto;

public class CreateProductDto :  IValidatableObject
{
    [Required(ErrorMessage = "Name is required")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Category is required")]
    public CategoryEnum Category { get; set; }
    
    [Required(ErrorMessage = "Perishable is required")]
    public bool Perishable { get; set; }
    
    public ICollection<int> SubCategories { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (!System.Enum.IsDefined(typeof(CategoryEnum), Category))
        {
            yield return new ValidationResult("Invalid Category value.", [nameof(Category)]);
        }
    }
}