using BusinessLogicLayer.Interfaces;
using DataAccessLayer.Interface;
using Model.DTO;
using Model.Entity;

namespace BusinessLogicLayer;

public class RoomService : IRoomService
{
    private readonly IGenericRepository<Room> _roomRepository;
    private readonly IGenericRepository<Promotion> _promotionRepository;
    private readonly IGenericRepository<Image> _imageRepository;
    public RoomService(IGenericRepository<Room> roomRepository, IGenericRepository<Promotion> promotionRepository, IGenericRepository<Image> imageRepository)
    {
        _roomRepository = roomRepository;
        _promotionRepository = promotionRepository;
        _imageRepository = imageRepository;
    }
    public async Task<ResultDTO<ICollection<RoomResponse>>> GetRooms()
    {
        ICollection<RoomResponse> roomResponses = new List<RoomResponse>(); 
        var rooms = await _roomRepository.GetAllAsync();
        foreach (var room in rooms)
        {
            RoomResponse roomResponse = new RoomResponse()
            {
                Address = room.Address,
                Capacity = room.Capacity,
                RoomId = room.RoomId,
                Description = room.Description,
                Status = room.Status,
                RoomName = room.RoomName,
                Price = room.Price
            };
            var promotion = await _promotionRepository.GetByProperty(x => x.RoomId == room.RoomId);
            if (promotion != null)
            {
                if (promotion.StartTime <= DateTime.Now && DateTime.Now <= promotion.EndTime)
                {
                    roomResponse.SalePrice = roomResponse.Price * promotion.ReductionPercent / 100;
                }
            }
            var image = await _imageRepository.GetByProperty(x => x.RoomId == room.RoomId);
            if (image != null)
            {
                roomResponse.imgPath = image.ImagePath;
            }
            roomResponses.Add(roomResponse);
        }

        ResultDTO<ICollection<RoomResponse>> result = new ResultDTO<ICollection<RoomResponse>>()
        {
            Data = roomResponses,
            isSuccess = true,
            Message = "Return rooms successfully"
        };
        return result;
    }
}