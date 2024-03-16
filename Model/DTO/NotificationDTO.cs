using Model.Entity;

namespace Model.DTO;

public class NotificationDTO
{
    public int NotificationId { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public DateTime SentTime { get; set; }
    public int UserId { get; set; }
}