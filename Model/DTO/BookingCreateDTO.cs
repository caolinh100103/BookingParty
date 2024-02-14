namespace Model.DTO;

public class BookingCreateDTO
{
    public DateTime StartTime { get; set; }
    public DateTime EndTIme { get; set; }
    public string? Ward { get; set; }
    public string? District { get; set; }
    public string? ExactAddress { get; set; }
    public decimal TotalPrice { get; set; }
    public int RoomId { get; set; }
    public IEnumerable<int> ServiceIds { get; set; }
}