using System.Collections;
using BusinessLogicLayer.Interfaces;
using AutoMapper;
using BusinessLogicLayer.Helper;
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

        public async Task<ResultDTO<ServiceDTO>> CreateService(ServiceCreatedDTO serviceCreatedDto)
        {
            ResultDTO<bool> result = null;
            ICollection<string> imageBase64Str = new List<string>();
            if (!serviceCreatedDto.Images.IsNullOrEmpty())
            {
                foreach (var image in serviceCreatedDto.Images)
                {
                    var imageString = Base64Converter.ConvertToBase64(image);
                    imageBase64Str.Add(imageString);
                }
            }

            var serviceMapper = new Service()
            {
                UserId = serviceCreatedDto.UserId,
                Price = serviceCreatedDto.Price,
                Description = serviceCreatedDto.Description,
                ServiceName = serviceCreatedDto.ServiceName,
                Status = 0,
                CategoryId = serviceCreatedDto.CategoryId,
                ServiceTitle = serviceCreatedDto.ServiceTitle
            };
            var service = await _serviceRepository.AddAsync(serviceMapper);
            if (service != null)
            {
                foreach (var str in imageBase64Str)
                {
                    Image image = new Image()
                    {
                        ServiceId = service.ServiceId,
                        ImageBase64 = str,
                        Status = 1,
                    };
                    _ = _imageRepository.AddAsync(image);
                }

                return new ResultDTO<ServiceDTO>()
                {
                    Data = null,
                    isSuccess = true,
                    Message = "Created service successfully"
                };
            }

            return new ResultDTO<ServiceDTO>()
            {
                Data = null,
                isSuccess = false,
                Message = "Can not created service successfully"
            };
        }

        public async Task<ResultDTO<bool>> DisableService(int serviceId)
        {
            var service = await _serviceRepository.GetByIdAsync(serviceId);
            if (service != null && service.Status == 1)
            {
                service.Status = 0;
                _ = await _serviceRepository.UpdateAsync(service);
                return new ResultDTO<bool>()
                {
                    Data = true,
                    isSuccess = true,
                    Message = "Disable service successfully"
                };
            }

            if (service.Status == 0)
            {
                return new ResultDTO<bool>()
                {
                    Data = true,
                    isSuccess = true,
                    Message = "The service has been disabled"
                };
            }

            return  new ResultDTO<bool>()
            {
                Data = true,
                isSuccess = true,
                Message = "Internal Server"
            };
        }
        
        public async Task<bool> Update(ServiceUpdateDTO serviceUpdateDto)
        {
            ICollection<String> imageBase64Str = new List<String>();
            var service = await _serviceRepository.GetByIdAsync(serviceUpdateDto.ServiceId);
            if (service == null)
            {
                return false;
            }
            if (service != null)
            {
                service.ServiceTitle = serviceUpdateDto.ServiceTitle;
                service.ServiceName = serviceUpdateDto.ServiceName;
                service.Description = serviceUpdateDto.Description;
                service.UserId = serviceUpdateDto.UserId;
                service.Price = serviceUpdateDto.Price;
                service.CategoryId = serviceUpdateDto.CategoryId;

                _ = await _serviceRepository.UpdateAsync(service);
            }
            else if (!serviceUpdateDto.Images.IsNullOrEmpty() && service != null)
            {
                foreach (var image in serviceUpdateDto.Images)
                {
                    var imageString = Base64Converter.ConvertToBase64(image.Image);
                    var imageGet = await _imageRepository.GetByIdAsync(image.ImageId);
                    imageGet.ImageBase64 = imageString;
                    _ = _imageRepository.UpdateAsync(imageGet);
                }
            }

            return true;
        }

        public async Task<ResultDTO<ICollection<ServiceResponseDTO>>> GetAllServiceBypartyHost(int partyHostId)
        {
            ICollection<ServiceResponseDTO> response = new List<ServiceResponseDTO>();
            var services = await _serviceRepository.GetListByProperty(x => x.UserId == partyHostId);
            foreach (var service in services)
            {
                var serviceMapper = _mapper.Map<ServiceResponseDTO>(service);
                response.Add(serviceMapper);
            }

            return new ResultDTO<ICollection<ServiceResponseDTO>>()
            {
                Data = response,
                isSuccess = true,
                Message = "Return list of services Bu party host"
            };
        }

        public async Task<ICollection<ServiceResponseDTO>> GetAllServices()
        {
            var services = await _serviceRepository.GetAllAsync();
            ICollection<ServiceResponseDTO> response = new List<ServiceResponseDTO>();
            ICollection<ImageDTO> imageDtos = new List<ImageDTO>();
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
                if (!images.IsNullOrEmpty())
                {
                    foreach (var image in images)
                    {
                        var iamgeMapper = _mapper.Map<ImageDTO>(image);
                        imageDtos.Add(iamgeMapper);
                    }

                    serviceResponse.Images = imageDtos;
                }
                response.Add(serviceResponse);
            }

            return response;
        }

        public async Task<ICollection<ServiceResponseDTO>> GetAllServicesWithPaging(int page, int pageSize)
        {
            var servicesPages = await _serviceRepository.GetPaginatedListAsync(page, pageSize);
            ICollection<ServiceResponseDTO> response = new List<ServiceResponseDTO>();
            ICollection<String> imageStr = new List<String>();
            ICollection<ImageDTO> imageDtos = new List<ImageDTO>();
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
                if (!images.IsNullOrEmpty())
                {
                    foreach (var image in images)
                    {
                        var iamgeMapper = _mapper.Map<ImageDTO>(image);
                        imageDtos.Add(iamgeMapper);
                    }

                    serviceResponse.Images = imageDtos;
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
