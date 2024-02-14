using AutoMapper;
using BusinessLogicLayer.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Model.DTO;
using Model.Entity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BookingPartyproject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServiceController : ControllerBase
    {
        private readonly IServicesService _service;
        private readonly IMapper _mapper;
        public ServiceController(IServicesService service, IMapper mapper) 
        {
            _service = service;
            _mapper = mapper;
        }

        [HttpGet("services")]
        [ProducesResponseType(200, Type = typeof(ServiceDTO))]
        [AllowAnonymous]
        public async Task<IActionResult> Services ()
        {
            var services = await _service.GetAllServices();
            var serviceDTOs = _mapper.Map<ICollection<ServiceDTO>>(services);
            ResultDTO<ICollection<ServiceDTO>> servicesResult = new ResultDTO<ICollection<ServiceDTO>>
            {
                Data = serviceDTOs,
                isSuccess = true,
                Message = "Return List of service successfully"
            };
            return Ok(servicesResult);
        }

        [HttpPost]
        [Authorize(Roles = "Party Host")]
        public async Task<IActionResult> CreateService([FromBody] ServiceDTO ServiceDto)
        {
            int checkCreate = await _service.CreateService(ServiceDto);
            if (checkCreate > 0)
            {
                ResultDTO<ServiceDTO> resultDto = new ResultDTO<ServiceDTO>
                {
                    Data = ServiceDto,
                    isSuccess = true,
                    Message = "Created a new service successfully"
                };
                return Ok(resultDto);
            }
            else
            {
                ResultDTO<ServiceDTO> resultDto = new ResultDTO<ServiceDTO>
                {
                    isSuccess = false,
                    Message = "Can not created"
                };
                return Ok(resultDto);
            }
        }
    }
}
