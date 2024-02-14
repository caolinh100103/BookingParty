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
    public IActionResult CreatePaymentUrl(BookingCreateDTO bookingDto) // sua lai thanh int bookingId vi tao booking roi moi deposit
    {
        var url = _vnPayService.CreatePaymentUrl(bookingDto, HttpContext);
            
        return Ok(Redirect(url));
    }
    // [HttpPost("/url_callback")]
    // public IActionResult PaymentCallback()
    // {
    //     var response = _vnPayService.PaymentExecute(Request.Query);
    //     return 
    // }
}