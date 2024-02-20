 using Model.Entity;

 namespace Model.DTO;

public class BookingResponseDTO
{
    public int BookingId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTIme { get; set; }
    public IEnumerable<ServiceDTO>? Services { get; set; }
    public RoomDTO? Room { get; set; }
}