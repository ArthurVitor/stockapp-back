using StockApp.Models.Dtos.SubCategoryDto;
using StockApp.Models.Enum;

namespace StockApp.Models.Dtos.ProductDto;

public class ListProductDTO
{
    public int Id { get; set; }
    
    public string Name { get; set; } = string.Empty;
    
    public CategoryEnum Category { get; set; }

    public bool Perishable { get; set; }
    
    public ICollection<ListSubCategoryDto>? SubCategories { get; set; }
}