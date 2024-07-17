namespace StockApp.Models.Dtos.Pagination;

public class PagedResultDto<T>(int totalRecords, IEnumerable<T> records)
{
    public int TotalRecords { get; set; } = totalRecords;
    public IEnumerable<T> Records { get; set; } = records;
}
