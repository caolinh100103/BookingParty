using BusinessLogicLayer.Helper;
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
    public  IActionResult CreatePaymentUrl(VNPayCreatedDTO vnPayCreatedDto) 
    {
        var url =  _vnPayService.CreatePaymentUrl(vnPayCreatedDto, HttpContext);
            
        return Ok(Redirect(url));
    }
    [HttpGet("IPN")]
    public async Task<IActionResult> GetPayment()
    {
        // var vnPayData = Request.QueryString;
        // VNPayHelper helper = new VNPayHelper();
        // foreach (var s in vnPayData)
        // {
        //     
        // }
        VnPayResponseDTO vnPayResponseDto = new VnPayResponseDTO
        {
            BookingId = HttpContext.Request.Query["vnp_TxnRef"].ToString(),
            PayDate = HttpContext.Request.Query["vnp_PayDate"].ToString(),
            Amount = HttpContext.Request.Query["vnp_Amount"].ToString(),
            PaymentMethod = HttpContext.Request.Query["vnp_BankCode"].ToString(),
            TransactionId = HttpContext.Request.Query["vnp_BankCode"].ToString(),
            ResponseCode = HttpContext.Request.Query["vnp_ResponseCode"].ToString()
            // BookingId = 55,
            // PayDate = "20220304",
            // Amount = 100000,
            // PaymentMethod = "VNPAY",
            // ResponseCode = "00",
            // TransactionId = "605154"
        };
        if (vnPayResponseDto.ResponseCode.Equals("00"))
        {
            await _vnPayService.UpdatePayment(vnPayResponseDto);
            return Ok();
        }

        return BadRequest();
    }
}