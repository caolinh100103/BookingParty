namespace Model.DTO;

public class RoomResponse
{
    public int RoomId{ get; set; }
    public string RoomName { get; set; }
    public string Description { get; set; }
    public int Capacity { get; set; }
    public string Address { get; set; }
    public int Status { get; set; }
    public ICollection<ImageDTO>? Images { get; set; }
    public decimal Price { get; set; }
    public decimal SalePrice {get; set; }
    public float Area { get; set; }
    public UserDTO User { get; set; }
    public ICollection<FeedbackReponseDTO>? Feedbacks { get; set; }
    public ICollection<FacilityRepsonseDTO>? Facilities { get; set; }
    public float AverageRating { get; set; }
    public int NumOfBookings { get; set; }
}