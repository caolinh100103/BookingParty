using BusinessLogicLayer.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using DataAccessLayer.Interface;
using Model.DTO;
using Model.Entity;

namespace BusinessLogicLayer
{
    public class ServicesService : IServicesService
    {
        private IGenericRepository<Service> _serviceRepository;
        private IMapper _mapper;

        public ServicesService (IGenericRepository<Service> serviceRepository, IMapper mapper)
        {
            _serviceRepository = serviceRepository;
            _mapper = mapper;
        }

        public async Task<int> CreateService(ServiceDTO serviceDto)
        {
            var service = _mapper.Map<Service>(serviceDto);
            var serviceCreated = await _serviceRepository.AddAsync(service);
            var checkCreated = (serviceCreated != null) ? 1 : 0;
            return checkCreated;
        }

        public async Task<ICollection<Service>> GetAllServices()
        {
            return await _serviceRepository.GetAllAsync();
        }

        public Service GetServicebyId(int Id)
        {
            throw new NotImplementedException();
        }
    }
}
