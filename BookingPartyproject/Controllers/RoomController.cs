using BusinessLogicLayer.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BookingPartyproject.Controllers;
[Route("api/[controller]")]
[ApiController]
public class RoomController : ControllerBase
{
    private readonly IRoomService _roomService;
    public RoomController(IRoomService roomService)
    {
        _roomService = roomService;
    }
    
    [HttpGet]
    public async Task<IActionResult> getRooms()
    {
        var response = await _roomService.GetRooms();
        if (response == null)
        {
            return BadRequest();
        }
        if (!response.isSuccess)
        {
            return BadRequest();
        }

        return Ok(response);
    }
}