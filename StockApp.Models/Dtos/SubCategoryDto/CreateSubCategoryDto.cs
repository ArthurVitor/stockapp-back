using System.ComponentModel.DataAnnotations;

namespace StockApp.Models.Dtos.SubCategoryDto;

public class CreateSubCategoryDto
{
    [Required]
    public string? Name { get; set; }
}
