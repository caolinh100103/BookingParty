namespace Model.DTO;

public class ServiceResponseDTO
{
    public int ServiceId { get; set; }
    public string ServiceTitle { get; set; }
    public string ServiceName { get; set; }
    public decimal Price { get; set; }
    public string Description { get; set; }
    public int Status { get; set; }
    public UserDTO User { get; set; }
    public int CategoryId { get; set; }
    public ICollection<ImageDTO> Images { get; set; }
    public ICollection<FeedbackReponseDTO>? Feedbacks { get; set; }
    public decimal Sale_Price { get; set; }
    public float AverageRating { get; set; }
    public int NumOfBookings { get; set; }
}