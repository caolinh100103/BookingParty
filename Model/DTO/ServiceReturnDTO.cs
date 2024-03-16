using Microsoft.AspNetCore.Http;

namespace Model.DTO;

public class ServiceReturnDTO
{
    public int ServiceId { get; set; }
    public string ServiceTitle { get; set; }
    public string ServiceName { get; set; }
    public decimal Price { get; set; }
    public string Description { get; set; }
    public CategoryDTO Category { get; set; }
    public List<string> ImagesBase64 { get; set; }
}