using Model.DTO;
using Model.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Interfaces
{
    public interface IServicesService
    {
        public Task<ICollection<ServiceResponseDTO>> GetAllServices();
        public Task<ICollection<ServiceResponseDTO>> GetAllServicesWithPaging(int page, int pageSize);
        public Service GetServicebyId(int Id);
        public Task<ResultDTO<ServiceDTO>> CreateService (ServiceCreatedDTO service);
        public Task<ResultDTO<bool>> DisableService(int serviceId);
        public Task<bool> Update(ServiceUpdateDTO serviceCreatedDto);
        public Task<ResultDTO<ICollection<ServiceResponseDTO>>> GetAllServiceBypartyHost(int partyHostId);
    }
}
