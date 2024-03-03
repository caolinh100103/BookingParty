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
            ResultDTO<ICollection<ServiceResponseDTO>> servicesResult = new ResultDTO<ICollection<ServiceResponseDTO>>
            {
                Data = services,
                isSuccess = true,
                Message = "Return List of service successfully"
            };
            return Ok(servicesResult);
        }
        [HttpGet("servicesPage")]
        [ProducesResponseType(200, Type = typeof(ServiceDTO))]
        [AllowAnonymous]
        public async Task<IActionResult> Services (int page, int pageSize)
        {
            var services = await _service.GetAllServicesWithPaging(page, pageSize);
            ResultDTO<ICollection<ServiceResponseDTO>> servicesResult = new ResultDTO<ICollection<ServiceResponseDTO>>
            {
                Data = services,
                isSuccess = true,
                Message = "Return List of service successfully"
            };
            return Ok(servicesResult);
        }

        [HttpPost]
        [Authorize(Roles = "Party Host")]
        public async Task<IActionResult> CreateService([FromForm] ServiceCreatedDTO ServiceDto)
        {
            var checkCreate = await _service.CreateService(ServiceDto);
            if (checkCreate.isSuccess == true)
            {
                return Ok(checkCreate);
            }
            else
            {
                return Ok(checkCreate);
            }
        }
        // [HttpPut]
        // [Authorize(Roles = "Admin")]
        // public async Task<IActionResult> ApproveService([FromBody] int ServiceId)
        // {
        //     
        // }

        [HttpDelete]
        public async Task<IActionResult> DisableService([FromBody] int ServiceId)
        {
            var result = await _service.DisableService(ServiceId);
            return Ok(result);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateService([FromForm] ServiceUpdateDTO serviceCreatedDto)
        {
            var result = await _service.Update(serviceCreatedDto);
            if (result)
            {
                return Ok(new ResultDTO<String>()
                {
                    Data = null,
                    isSuccess = true,
                    Message = "Update Successfully"
                });
            }
            else
            {
                return Ok(new ResultDTO<String>()
                {
                    Data = null,
                    isSuccess = false,
                    Message = "Update Not Successfully"
                });
            }
        }
    }
}
