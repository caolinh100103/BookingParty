namespace Model.DTO;

public class BookingDetailDTO
{
    public DateTime StartTime { get; set; }
    public DateTime EndTIme { get; set; }
    public int BookingId { get; set; }
    public int ServiceId { get; set; }
}