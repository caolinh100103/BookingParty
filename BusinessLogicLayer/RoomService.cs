using AutoMapper;
using BusinessLogicLayer.Interfaces;
using DataAccessLayer.Interface;
using Microsoft.IdentityModel.Tokens;
using Model.DTO;
using Model.Entity;

namespace BusinessLogicLayer;

public class RoomService : IRoomService
{
    private readonly IGenericRepository<Room> _roomRepository;
    private readonly IGenericRepository<Promotion> _promotionRepository;
    private readonly IGenericRepository<Image> _imageRepository;
    private readonly IGenericRepository<Facility> _facilityRepository;
    private readonly IGenericRepository<Feedback> _feedbackRepository;
    private readonly IMapper _mapper;
    public RoomService(IGenericRepository<Room> roomRepository, IGenericRepository<Promotion> promotionRepository, IGenericRepository<Image> imageRepository,
        IGenericRepository<Facility> facilityRepository, IMapper mapper, IGenericRepository<Feedback> feedbackRepository)
    {
        _roomRepository = roomRepository;
        _promotionRepository = promotionRepository;
        _imageRepository = imageRepository;
        _facilityRepository = facilityRepository;
        _mapper = mapper;
        _feedbackRepository = feedbackRepository;
    }
    public async Task<ResultDTO<ICollection<RoomResponse>>> GetRooms()
    {
        ICollection<RoomResponse> roomResponses = new List<RoomResponse>(); 
        ICollection<FeedbackReponseDTO> feedbackReponseDtos = new List<FeedbackReponseDTO>();
        var rooms = await _roomRepository.GetAllAsync();
        ICollection<FacilityRepsonseDTO> facilityRepsonseDtos = new List<FacilityRepsonseDTO>();
        RoomResponse roomResponse = null;
        foreach (var room in rooms)
        {
            var facilities = await _facilityRepository.GetListByProperty(x => x.RoomId == room.RoomId);
            if (!facilities.IsNullOrEmpty())
            {
                foreach (var facility in facilities)
                {
                    var facilityMapper = _mapper.Map<FacilityRepsonseDTO>(facility);
                    facilityRepsonseDtos.Add(facilityMapper);
                }
                roomResponse.Facilities = facilityRepsonseDtos;
            }
            var feedbacks = await _feedbackRepository.GetListByProperty(x => x.RoomId == room.RoomId);
            if (!feedbacks.IsNullOrEmpty())
            {
                foreach (var feedback in feedbacks)
                {
                    var feedbackMapper = _mapper.Map<FeedbackReponseDTO>(feedback);
                    feedbackReponseDtos.Add(feedbackMapper);
                }
                roomResponse.Feedbacks = feedbackReponseDtos;
            }
            roomResponse = new RoomResponse()
            {
                Address = room.Address,
                Capacity = room.Capacity,
                RoomId = room.RoomId,
                Description = room.Description,
                Status = room.Status,
                RoomName = room.RoomName,
                Price = room.Price,
                Facilities = facilityRepsonseDtos
            };
            var promotion = await _promotionRepository.GetByProperty(x => x.RoomId == room.RoomId);
            if (promotion != null)
            {
                if (promotion.StartTime <= DateTime.Now && DateTime.Now <= promotion.EndTime)
                {
                    roomResponse.SalePrice = roomResponse.Price * promotion.ReductionPercent / 100;
                }
            }
            var images = await _imageRepository.GetListByProperty(x => x.RoomId == room.RoomId);
            if (!images.IsNullOrEmpty())
            {
                foreach (var image in images)
                {
                    roomResponse.ImagePaths.Add(image.ImagePath);
                }
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