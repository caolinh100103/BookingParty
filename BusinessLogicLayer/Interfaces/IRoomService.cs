using Model.DTO;

namespace BusinessLogicLayer.Interfaces;

public interface IRoomService
{
    Task<ResultDTO<ICollection<RoomResponse>>> GetRooms();
    Task<ResultDTO<ICollection<RoomResponse>>> GetRoomsWithPaging(int page, int pageSize);
}
