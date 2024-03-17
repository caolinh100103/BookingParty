namespace BusinessLogicLayer.Interfaces;

public interface ISSEService
{
    Task SendNotification(string Message);
    Task SendNotification(string userId, object data);
    void AddConnection(int userId, StreamWriter streamWriter);
    void RemoveConnection(int userId);
    Task SendNotificationToUserAsync(int userId, string notificationMessage);
}