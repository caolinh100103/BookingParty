namespace Model.DTO;

public class ServiceDetailDTO
{
    public int ServiceId { get; set; }
    public string ServiceTitle { get; set; }
    public string ServiceName { get; set; }
    public decimal Price { get; set; }
    public string Description { get; set; }
    public int Status { get; set; }
    public UserDTO User { get; set; }
    public int CategoryId { get; set; }
}