using Model.DTO;

namespace BusinessLogicLayer.Interfaces;

public interface IRoomService
{
    Task<ResultDTO<ICollection<RoomResponse>>> GetRooms();
}
