namespace StockApp.API.Exceptions;

public class SubCategoryDoesntExist : Exception
{
    public SubCategoryDoesntExist(string message) : base(message)
    {
        
    }
}