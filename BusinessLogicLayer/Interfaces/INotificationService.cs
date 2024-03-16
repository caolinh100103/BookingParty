using Model.DTO;

namespace BusinessLogicLayer.Interfaces;

public interface INotificationService
{
    Task<ResultDTO<ICollection<NotificationResponseDTO>>> getNotiByid(int UserId);
}