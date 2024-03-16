namespace DataAccessLayer.Common;

public class PaginatedResult<T>
{
    public List<T> Items { get; set; }
    public int TotalCount { get; set; }
    public int PageCount { get; set; }
}