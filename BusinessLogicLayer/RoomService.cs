using System.Collections;
using System.Diagnostics.Contracts;
using AutoMapper;
using BusinessLogicLayer.Helper;
using BusinessLogicLayer.Interfaces;
using DataAccessLayer.Interface;
using Microsoft.IdentityModel.Tokens;
using Model.DTO;
using Model.Entity;
using String = System.String;

namespace BusinessLogicLayer;

public class RoomService : IRoomService
{
    private readonly IGenericRepository<Room> _roomRepository;
    private readonly IGenericRepository<Promotion> _promotionRepository;
    private readonly IGenericRepository<Image> _imageRepository;
    private readonly IGenericRepository<Facility> _facilityRepository;
    private readonly IGenericRepository<Feedback> _feedbackRepository;
    private IGenericRepository<BookingDetail> _bookingDetailrepository;
    private readonly IMapper _mapper;
    public RoomService(IGenericRepository<Room> roomRepository, IGenericRepository<Promotion> promotionRepository, IGenericRepository<Image> imageRepository,
        IGenericRepository<Facility> facilityRepository, IMapper mapper, IGenericRepository<Feedback> feedbackRepository,
        IGenericRepository<BookingDetail> bookingDetailrepository)
    {
        _roomRepository = roomRepository;
        _promotionRepository = promotionRepository;
        _imageRepository = imageRepository;
        _facilityRepository = facilityRepository;
        _mapper = mapper;
        _feedbackRepository = feedbackRepository;
        _bookingDetailrepository = bookingDetailrepository;
    }
    public async Task<ResultDTO<ICollection<RoomResponse>>> GetRooms()
    {
        ICollection<RoomResponse> roomResponses = new List<RoomResponse>(); 
        ICollection<String> imageStr = new List<String>();
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
                    imageStr.Add(image.ImageBase64);
                }

                roomResponse.Images = imageStr;
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

    public async Task<ResultDTO<ICollection<RoomResponse>>> GetRoomsWithPaging(int page, int pageSize)
    {
        ICollection<RoomResponse> roomResponses = new List<RoomResponse>(); 
        ICollection<String> imageStr = new List<String>();
        ICollection<FeedbackReponseDTO> feedbackReponseDtos = new List<FeedbackReponseDTO>();
        var roomsPage = await _roomRepository.GetPaginatedListAsync(page, pageSize);
        ICollection<FacilityRepsonseDTO> facilityRepsonseDtos = new List<FacilityRepsonseDTO>();
        RoomResponse roomResponse = null;
        foreach (var room in roomsPage.Items)
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
                    imageStr.Add(image.ImageBase64);
                }

                roomResponse.Images = imageStr;
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

    public async Task<ResultDTO<int>> CreateRoom(RoomCreatedDTO roomCreatedDto)
    {
        ResultDTO<int> result = null;
        ICollection<string> imageBase64Str = new List<string>();
        if (!roomCreatedDto.Images.IsNullOrEmpty())
        {
            foreach (var image in roomCreatedDto.Images)
            {
                var imageString = Base64Converter.ConvertToBase64(image);
                imageBase64Str.Add(imageString);
            }
        }

        var roomMapper = new Room()
        {
            Address = roomCreatedDto.Address,
            Capacity = roomCreatedDto.Capacity,
            Description = roomCreatedDto.Description,
            RoomName = roomCreatedDto.RoomName,
            UserId = roomCreatedDto.UserId,
            Status = 0
        };
        var room = await _roomRepository.AddAsync(roomMapper);
        if (room != null)
        {
            foreach (var str in imageBase64Str)
            {
                Image image = new Image()
                {
                    RoomId = room.RoomId,
                    ImageBase64 = str,
                    Status = 1,
                };
                _ = await _imageRepository.AddAsync(image);
            }
            return new ResultDTO<int>()
                {
                    Data = 1,
                    isSuccess = true,
                    Message = "Created room successfully"
                };

        }

        return new ResultDTO<int>()
        {
            Data = 0,
            isSuccess = false,
            Message = "Can not created service successfully"
        };
    }

    public async Task<ResultDTO<bool>> DisableRoom(int RoomId)
    {
        var room = await _roomRepository.GetByIdAsync(RoomId);
        if (room != null && room.Status == 1)
        {
            room.Status = 0;
            _ = await _roomRepository.UpdateAsync(room);
            return new ResultDTO<bool>()
            {
                Data = true,
                isSuccess = true,
                Message = "Disable Successfully"
            };
        }

        if (room.Status == 0)
        {
            return new ResultDTO<bool>()
            {
                Data = false,
                isSuccess = false,
                Message = "The Room has been disabled"
            };
        }

        return new ResultDTO<bool>()
        {
            Data = false,
            isSuccess = false,
            Message = "Internal Server error"
        };
    }

    public async Task<ResultDTO<ICollection<RoomResponse>>> FindAvailableRoomInDateTime(SearchAvalableRoomDTO searchAvalableRoomDto)
    {
        if (searchAvalableRoomDto.EndTime < searchAvalableRoomDto.StartTime)
        {
            return new ResultDTO<ICollection<RoomResponse>>()
            {
                Data = null,
                isSuccess = false,
                Message = "can not search rooms with start time > end time"
            };
        }
        var hours = (searchAvalableRoomDto.EndTime - searchAvalableRoomDto.StartTime).TotalHours;
        if (hours > 4)
        {
            return new ResultDTO<ICollection<RoomResponse>>()
            {
                Data = null,
                isSuccess = false,
                Message = "Please choose the difference between startTime and Endtime is 4 hours"
            };
        }
        ICollection<RoomResponse> roomResponses = null;
        var rooms = await _roomRepository.GetAllAsync();
        var bookingDetailList = await _bookingDetailrepository.GetListByProperty(x =>
            (x.StartTime <= searchAvalableRoomDto.EndTime &&
              x.EndTIme >= searchAvalableRoomDto.StartTime));
        if (bookingDetailList.IsNullOrEmpty())
        {
            roomResponses = new List<RoomResponse>();
            foreach (var room in rooms)
            {
                var roomMapper = _mapper.Map<RoomResponse>(room);
                roomResponses.Add(roomMapper);
            }

            return new ResultDTO<ICollection<RoomResponse>>
            {
                Data = roomResponses,
                isSuccess = true,
                Message = "Return all rooms"
            };
        }
        else
        {
            foreach (var bookingDetail in bookingDetailList)
            {
                var roomGet = await _roomRepository.GetByProperty(x => x.RoomId == bookingDetail.RoomId);
                rooms.Remove(roomGet);
            }
            roomResponses = new List<RoomResponse>();
            foreach (var room in rooms)
            {
                var roomMapper = _mapper.Map<RoomResponse>(room);
                roomResponses.Add(roomMapper);
            }
            return new ResultDTO<ICollection<RoomResponse>>()
            {
                Data = roomResponses,
                isSuccess = true,
                Message = "Return List of room after filter"
            };
        }
    }

    public async Task<ResultDTO<ICollection<RoomResponse>>> GetAllRoomsByPartyHostId(int partyHostId)
    {
        ICollection<RoomResponse> response = new List<RoomResponse>();
        var rooms = await _roomRepository.GetListByProperty(x => x.UserId == partyHostId);
        if (!rooms.IsNullOrEmpty())
        {
            foreach (var room in rooms)
            {
                var roomMapper = _mapper.Map<RoomResponse>(room);
                response.Add(roomMapper);
            }

            return new ResultDTO<ICollection<RoomResponse>>()
            {
                Data = response,
                isSuccess = true,
                Message = "return list of rooms"
            };
        }

        return new ResultDTO<ICollection<RoomResponse>>()
        {
            Data = null,
            isSuccess = true,
            Message = "The party host does not have any room"
        };
    }
}