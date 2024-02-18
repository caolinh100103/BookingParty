namespace Model.DTO;

public class ServiceResponseDTO
{
    public int ServiceId { get; set; }
    public string ServiceName { get; set; }
    public decimal Price { get; set; }
    public string Description { get; set; }
    public int Status { get; set; }
    public int UserId { get; set; }
    public int CategoryId { get; set; }
    public string ImagePath { get; set; }
    public decimal Sale_Price { get; set; }
}