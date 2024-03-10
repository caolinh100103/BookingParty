using Model.DTO;
using Model.Entity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Interfaces
{
    public interface IServicesService
    {
        Task<ICollection<ServiceResponseDTO>> GetAllServices();
        Task<ICollection<ServiceResponseDTO>> GetAllServicesWithPaging(int page, int pageSize);
        Service GetServicebyId(int Id);
        Task<ResultDTO<ServiceDTO>> CreateService (ServiceCreatedDTO service);
        Task<ResultDTO<bool>> DisableService(int serviceId);
        Task<bool> Update(int serviceId, ServiceUpdateDTO serviceCreatedDto);
        Task<ResultDTO<ICollection<ServiceResponseDTO>>> GetAllServiceBypartyHost(int partyHostId);
        Task<ResultDTO<ICollection<ServiceResponseDTO>>> SearchService(string searchItem);
    }
}
