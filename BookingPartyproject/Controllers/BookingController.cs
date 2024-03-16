using BusinessLogicLayer.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Model.DTO;

namespace BookingPartyproject.Controllers;

[Route("api/[controller]")]
[ApiController]
public class BookingController : ControllerBase
{
    private readonly IBookingService _bookingService;
    public BookingController(IBookingService bookingService)
    {
        _bookingService = bookingService;
    }
    
    [HttpPost]
    [Authorize(Roles = "Customer")]
    public async Task<IActionResult> CreateBooking([FromBody] BookingCreateDTO bookingDto)
    {
        string tokenAuth = Request.Cookies["token"];
        if (string.IsNullOrEmpty(tokenAuth))
        {
            ResultDTO<string> response = new ResultDTO<string>
            {
                Data = null,
                isSuccess = false,
                Message = "Unauthorized"
            };
            return Unauthorized(response);
        }
        var bookingResponse = await _bookingService.CreateBooking(bookingDto, tokenAuth);
        return Ok(bookingResponse);
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAllBooking()
    {
        var result = await _bookingService.GetAllBooking();
        return Ok(result);
    }

    [HttpGet("bookingdetails")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAllBookingDetail(int bookingId)
    {
        var result = await _bookingService.GetAllBookingDetailByBookingId(bookingId);
        return Ok(result);
    }
    
    [HttpPost("CancelByCustomer")]
    [Authorize(Roles = "Customer")]
    public async Task<IActionResult> CancelByCustomer([FromBody] int BookingId)
    {
        var result = await _bookingService.CancelByCustomer(BookingId);
        return Ok(result);
    }

    [HttpPut("finishBooking")]
    [Authorize("Party Host")]
    public async Task<IActionResult> FinishBooking([FromBody] int bookingId)
    {
        var result = await _bookingService.FinishBooking(bookingId);
        return Ok(result);
    }

    [HttpGet("booking/{userId}")]
    [Authorize(Roles = "Customer")]
    public async Task<IActionResult> GetAllBookingByUserId(int userId)
    {
        var result = await _bookingService.GetAllBookingByUserId(userId);
        return Ok(result);
    }

    [HttpPost("CancelByPartyHost")]
    [Authorize(Roles = "Party Host")]
    public async Task<IActionResult> CancelByPartyHost([FromBody]int bookingId)
    {
        var result = await _bookingService.CancelByPartyHost(bookingId);
        return Ok(result);
    }

    [HttpGet("get_booking_partyhost")]
    [Authorize(Roles = "Party Host")]
    public async Task<IActionResult> GetBookingOfPartyHost()
    {
        string tokenAuth = Request.Cookies["token"];
        var result = await _bookingService.GetBookingByPartyHost(tokenAuth);
        return Ok(result);
    }
}