using BusinessLogicLayer.Interfaces;
using AutoMapper;
using DataAccessLayer.Interface;
using Microsoft.IdentityModel.Tokens;
using Model.DTO;
using Model.Entity;

namespace BusinessLogicLayer
{
    public class ServicesService : IServicesService
    {
        private readonly IGenericRepository<Service> _serviceRepository;
        private readonly IGenericRepository<Promotion> _promotionRepository;
        private readonly IGenericRepository<Image> _imageRepository;
        private readonly IGenericRepository<User> _userRepository;
        private readonly IGenericRepository<Feedback> _feedbackRepository;
        private readonly IMapper _mapper;

        public ServicesService (IGenericRepository<Service> serviceRepository, IGenericRepository<Promotion> promotionRepository,
            IGenericRepository<Image> imageRepository , IMapper mapper, IGenericRepository<User> userRepository,
            IGenericRepository<Feedback> feedbackRepository)
        {
            _serviceRepository = serviceRepository;
            _promotionRepository = promotionRepository;
            _imageRepository = imageRepository;
            _userRepository = userRepository;
            _feedbackRepository = feedbackRepository;
            _mapper = mapper;
        }

        public async Task<int> CreateService(ServiceDTO serviceDto)
        {
            var service = _mapper.Map<Service>(serviceDto);
            var serviceCreated = await _serviceRepository.AddAsync(service);
            var checkCreated = (serviceCreated != null) ? 1 : 0;
            return checkCreated;
        }

        public async Task<ICollection<ServiceResponseDTO>> GetAllServices()
        {
            var services = await _serviceRepository.GetAllAsync();
            ICollection<ServiceResponseDTO> response = new List<ServiceResponseDTO>();
            ICollection<FeedbackReponseDTO> feedbackReponseDtos = new List<FeedbackReponseDTO>();
            foreach (var service in services)
            {
                ServiceResponseDTO serviceResponse = new ServiceResponseDTO();
                var serviceMapper = _mapper.Map<ServiceDTO>(service);
                serviceResponse.ServiceId = serviceMapper.ServiceId;
                serviceResponse.Description = serviceMapper.Description;
                serviceResponse.ServiceName = serviceMapper.ServiceName;
                serviceResponse.Status = serviceMapper.Status;
                serviceResponse.CategoryId = serviceMapper.CategoryId;
                var user = await _userRepository.GetByIdAsync(service.UserId);
                if (user != null)
                {
                    var userMapper = _mapper.Map<UserDTO>(user);
                    serviceResponse.User = userMapper;
                }

                var feedbacks = await _feedbackRepository.GetListByProperty(x => x.ServiceId == service.ServiceId);
                if (!feedbacks.IsNullOrEmpty())
                {
                    foreach (var feedback in feedbacks)
                    {
                        var feedbackMapper = _mapper.Map<FeedbackReponseDTO>(feedback);
                        feedbackReponseDtos.Add(feedbackMapper);
                    }
                    serviceResponse.Feedbacks = feedbackReponseDtos;
                }

                serviceResponse.Feedbacks = feedbackReponseDtos;
                serviceResponse.Price = serviceMapper.Price;
                serviceResponse.ServiceTitle = serviceMapper.ServiceTitle;
                var promotion = await _promotionRepository.GetByProperty(x => x.ServiceId == service.ServiceId);
                if (promotion != null)
                {
                    if (promotion.StartTime <= DateTime.Now && DateTime.Now <= promotion.EndTime)
                    {
                        serviceResponse.Sale_Price = serviceResponse.Price * promotion.ReductionPercent / 100;
                    }
                }
                var images = await _imageRepository.GetListByProperty(x => x.ServiceId == service.ServiceId);
                if (images != null)
                {
                    foreach (var image in images)
                    {
                        serviceResponse.ImagePaths.Add(image.ImagePath);
                    }
                }
                response.Add(serviceResponse);
            }

            return response;
        }

        public async Task<ICollection<ServiceResponseDTO>> GetAllServicesWithPaging(int page, int pageSize)
        {
            var servicesPages = await _serviceRepository.GetPaginatedListAsync(page, pageSize);
            ICollection<ServiceResponseDTO> response = new List<ServiceResponseDTO>();
            ICollection<FeedbackReponseDTO> feedbackReponseDtos = new List<FeedbackReponseDTO>();
            foreach (var service in servicesPages.Items)
            {
                ServiceResponseDTO serviceResponse = new ServiceResponseDTO();
                var serviceMapper = _mapper.Map<ServiceDTO>(service);
                serviceResponse.ServiceId = serviceMapper.ServiceId;
                serviceResponse.Description = serviceMapper.Description;
                serviceResponse.ServiceName = serviceMapper.ServiceName;
                serviceResponse.Status = serviceMapper.Status;
                serviceResponse.CategoryId = serviceMapper.CategoryId;
                var user = await _userRepository.GetByIdAsync(service.UserId);
                if (user != null)
                {
                    var userMapper = _mapper.Map<UserDTO>(user);
                    serviceResponse.User = userMapper;
                }

                var feedbacks = await _feedbackRepository.GetListByProperty(x => x.ServiceId == service.ServiceId);
                if (!feedbacks.IsNullOrEmpty())
                {
                    foreach (var feedback in feedbacks)
                    {
                        var feedbackMapper = _mapper.Map<FeedbackReponseDTO>(feedback);
                        feedbackReponseDtos.Add(feedbackMapper);
                    }
                    serviceResponse.Feedbacks = feedbackReponseDtos;
                }

                serviceResponse.Feedbacks = feedbackReponseDtos;
                serviceResponse.Price = serviceMapper.Price;
                serviceResponse.ServiceTitle = serviceMapper.ServiceTitle;
                var promotion = await _promotionRepository.GetByProperty(x => x.ServiceId == service.ServiceId);
                if (promotion != null)
                {
                    if (promotion.StartTime <= DateTime.Now && DateTime.Now <= promotion.EndTime)
                    {
                        serviceResponse.Sale_Price = serviceResponse.Price * promotion.ReductionPercent / 100;
                    }
                }
                var images = await _imageRepository.GetListByProperty(x => x.ServiceId == service.ServiceId);
                if (images != null)
                {
                    foreach (var image in images)
                    {
                        serviceResponse.ImagePaths.Add(image.ImagePath);
                    }
                }
                response.Add(serviceResponse);
            }

            return response;
        }

        public Service GetServicebyId(int Id)
        {
            throw new NotImplementedException();
        }
    }
}
