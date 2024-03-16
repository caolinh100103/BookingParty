using Microsoft.AspNetCore.Http;

namespace Model.DTO;

public class ServiceUpdateDTO
{
    public string ServiceTitle { get; set; }
    public string ServiceName { get; set; }
    public decimal Price { get; set; }
    public string Description { get; set; }
    public int UserId { get; set; }
    public int CategoryId { get; set; }
    public ICollection<ImageUpdateDTO> Images { get; set; }
}