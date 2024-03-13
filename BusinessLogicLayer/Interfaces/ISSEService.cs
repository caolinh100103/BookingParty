namespace BusinessLogicLayer.Interfaces;

public interface ISSEService
{
    Task SendNotification(string Message);
    Task SendNotification(string userId, object data);
}