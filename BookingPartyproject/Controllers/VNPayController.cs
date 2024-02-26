using BusinessLogicLayer.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Model.DTO;

namespace BookingPartyproject.Controllers;
[Route("api/[controller]")]
[ApiController]
public class VNPayController : ControllerBase
{
    private readonly IVnPayService _vnPayService;

    public VNPayController(IVnPayService vnPayService)
    {
        _vnPayService = vnPayService;
    }
    [HttpPost]
    public IActionResult CreatePaymentUrl(VNPayCreatedDTO vnPayCreatedDto) 
    {
        var url = _vnPayService.CreatePaymentUrl(vnPayCreatedDto, HttpContext);
            
        return Ok(Redirect(url));
    }
}