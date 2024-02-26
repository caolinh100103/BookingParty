namespace Model.DTO;

public class FeedbackReponseDTO
{
    public int FeedbackId { get; set; }
    public int Rate { get; set; }
    public string Content { get; set; }
    public int Status { get; set; }
    public DateTime Created { get; set; }

    public int? RoomId { get; set; }
    public int? ServiceId { get; set; }
    public int UserId { get; set; }
}