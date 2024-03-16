namespace Model.DTO;

public class ImageDTO
{
    public string ImageBase64 { get; set; }
    public int Status { get; set; }
    
    public int? ServiceId { get; set; }
    public int? RoomId { get; set; }
}