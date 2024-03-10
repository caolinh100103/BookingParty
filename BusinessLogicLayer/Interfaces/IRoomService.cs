using Model.DTO;

namespace BusinessLogicLayer.Interfaces;

public interface IRoomService
{
    Task<ResultDTO<ICollection<RoomResponse>>> GetRooms();
    Task<ResultDTO<ICollection<RoomResponse>>> GetRoomsWithPaging(int page, int pageSize);
    Task<ResultDTO<int>> CreateRoom(RoomCreatedDTO roomCreatedDto);
    Task<ResultDTO<bool>> DisableRoom(int RoomId);
    Task<ResultDTO<ICollection<RoomResponse>>> FindAvailableRoomInDateTime(SearchAvalableRoomDTO searchAvalableRoomDto);
    Task<ResultDTO<ICollection<RoomResponse>>> GetAllRoomsByPartyHostId(int partyHostId);
    Task<ResultDTO<bool>> UpdateRoom(int roomId, RoomUpdatedDTO roomCreatedDto);
}
