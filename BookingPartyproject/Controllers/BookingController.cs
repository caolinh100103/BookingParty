using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
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
    // [HttpGet]
    // public async Task<IActionResult> GetBoookingByUserId
}