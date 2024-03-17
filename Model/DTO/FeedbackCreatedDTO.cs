namespace Model.DTO;

public class FeedbackCreatedDTO
{
    public int Rate { get; set; }
    public string Content { get; set; }
    public int? ServiceId { get; set; }
    public int? RoomId { get; set; } 
    public int UserId { get; set; }
}