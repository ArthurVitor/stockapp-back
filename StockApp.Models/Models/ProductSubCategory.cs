namespace StockApp.Models.Models;

public class ProductSubCategory
{
    public int ProductId { get; set; }
   
    public int SubCategoryId { get; set; }

    public Product Product { get; set; }
    public SubCategory SubCategory { get; set; }
}