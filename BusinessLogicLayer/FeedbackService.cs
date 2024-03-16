using AutoMapper;
using BusinessLogicLayer.Interfaces;
using DataAccessLayer.Interface;
using Model.DTO;
using Model.Entity;

namespace BusinessLogicLayer;

public class FeedbackService : IFeedbackService
{
    private readonly IGenericRepository<Feedback> _feedbackRepository;
    private readonly IGenericRepository<User> _userRepository;
    private readonly IMapper _mapper;
    public FeedbackService(IGenericRepository<Feedback> feedbackRepository, IMapper mapper,
        IGenericRepository<User> userRepository)
    {
        _feedbackRepository = feedbackRepository;
        _userRepository = userRepository;
        _mapper = mapper;
    }
    public async Task<ResultDTO<FeedbackReponseDTO>> CreateFeedback(FeedbackCreatedDTO feedbackCreatedDto)
    {
        FeedbackReponseDTO feedbackReponseDto = null;
        if (feedbackCreatedDto.ServiceId == null)
        {
            var feedback = _mapper.Map<Feedback>(feedbackCreatedDto);
            var feedbackCreated = await _feedbackRepository.AddAsync(feedback);
            if (feedbackCreated != null)
            {
                var user = await _userRepository.GetByProperty(x => x.UserId == feedbackCreated.UserId);
                var userMapper = _mapper.Map<UserDTO>(user);
                feedbackReponseDto = _mapper.Map<FeedbackReponseDTO>(feedbackCreated);
                feedbackReponseDto.User = userMapper;
                return new ResultDTO<FeedbackReponseDTO>()
                {
                    Data = feedbackReponseDto,
                    isSuccess = true,
                    Message = "Create feedback of serivce"
                };
            }
        }
        else if (feedbackCreatedDto.RoomId == null)
        {
            var feedback = _mapper.Map<Feedback>(feedbackCreatedDto);
            var feedbackCreated = await _feedbackRepository.AddAsync(feedback);
            if (feedbackCreated != null)
            {
                var user = await _userRepository.GetByProperty(x => x.UserId == feedbackCreated.UserId);
                var userMapper = _mapper.Map<UserDTO>(user);
                feedbackReponseDto = _mapper.Map<FeedbackReponseDTO>(feedbackCreated);
                feedbackReponseDto.User = userMapper;
                return new ResultDTO<FeedbackReponseDTO>()
                {
                    Data = feedbackReponseDto,
                    isSuccess = true,
                    Message = "Create feedback of room"
                };
            }
        }

        return new ResultDTO<FeedbackReponseDTO>()
        {
            Data = null,
            isSuccess = false,
            Message = "Internal Server"
        };
    }
}