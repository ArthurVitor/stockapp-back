using System.ComponentModel.DataAnnotations;
using StockApp.Models.Enum;

namespace StockApp.Models.Dtos.ProductDto;

public class CreateImportingProductDto :  IValidatableObject
{
    public string Name { get; set; } = string.Empty;
    
    public CategoryEnum Category { get; set; }
    
    public bool Perishable { get; set; }
    
    public ICollection<string> SubCategories { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (!System.Enum.IsDefined(typeof(CategoryEnum), Category))
        {
            yield return new ValidationResult("Invalid Category value.", [nameof(Category)]);
        }
    }
}