using Model.Entity;

namespace Model.DTO;

public class NotificationResponseDTO
{
    public int NotificationId { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public DateTime SentTime { get; set; }
    public virtual User User { get; set; }
}