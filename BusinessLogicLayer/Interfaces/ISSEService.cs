namespace BusinessLogicLayer.Interfaces;

public interface ISSEService
{
    Task SendNotification(string Message);
}