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
        public Service GetServicebyId(int Id);
        public Task<int> CreateService (ServiceDTO service);
    }
}
