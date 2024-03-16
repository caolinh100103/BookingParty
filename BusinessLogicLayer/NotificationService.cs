using AutoMapper;
using BusinessLogicLayer.Interfaces;
using DataAccessLayer.Interface;
using Model.DTO;
using Model.Entity;

namespace BusinessLogicLayer;

public class NotificationService : INotificationService
{
    private readonly IGenericRepository<Notification> _notificationRepository;
    private readonly IMapper _mapper;
    public NotificationService(IGenericRepository<Notification> notificationRepository,
        IMapper mapper)
    {
        _notificationRepository = notificationRepository;
        _mapper = mapper;
    }
    public async Task<ResultDTO<ICollection<NotificationResponseDTO>>> getNotiByid(int UserId)
    {
        var notifications = await _notificationRepository.GetListByProperty(x => x.UserId == UserId);
        if (notifications != null)
        {
            ICollection<NotificationResponseDTO> notificationResponseDtos = new List<NotificationResponseDTO>();
            foreach (var noti in notifications)
            {
                var notiResponse = _mapper.Map<NotificationResponseDTO>(noti);
                notificationResponseDtos.Add(notiResponse);
            }
            var response = new ResultDTO<ICollection<NotificationResponseDTO>>()
            {
                Data = notificationResponseDtos,
                isSuccess = true,
                Message = "Return Notifications of an user"
            };
            return response;
        }
        else
        {
            var response = new ResultDTO<ICollection<NotificationResponseDTO>>()
            {
                Data = null,
                isSuccess = true,
                Message = "The user does not have any noti"
            };
            return response;
        }
    }
}