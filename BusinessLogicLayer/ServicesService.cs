using BusinessLogicLayer.Interfaces;
using AutoMapper;
using DataAccessLayer.Interface;
using Model.DTO;
using Model.Entity;

namespace BusinessLogicLayer
{
    public class ServicesService : IServicesService
    {
        private readonly IGenericRepository<Service> _serviceRepository;
        private readonly IGenericRepository<Promotion> _promotionRepository;
        private readonly IGenericRepository<Image> _imageRepository;
        private readonly IMapper _mapper;

        public ServicesService (IGenericRepository<Service> serviceRepository, IGenericRepository<Promotion> promotionRepository,
            IGenericRepository<Image> imageRepository , IMapper mapper)
        {
            _serviceRepository = serviceRepository;
            _promotionRepository = promotionRepository;
            _imageRepository = imageRepository;
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
            foreach (var service in services)
            {
                ServiceResponseDTO serviceResponse = new ServiceResponseDTO();
                var serviceMapper = _mapper.Map<ServiceDTO>(service);
                serviceResponse.Service = serviceMapper;
                var promotion = await _promotionRepository.GetByProperty(x => x.ServiceId == service.ServiceId);
                if (promotion != null)
                {
                    if (promotion.StartTime <= DateTime.Now && DateTime.Now <= promotion.EndTime)
                    {
                        serviceResponse.Sale_Price = serviceResponse.Service.Price * promotion.ReductionPercent / 100;
                    }
                }
                var image = await _imageRepository.GetByProperty(x => x.ServiceId == service.ServiceId);
                if (image != null)
                {
                    serviceResponse.ImagePath = image.ImagePath;
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
