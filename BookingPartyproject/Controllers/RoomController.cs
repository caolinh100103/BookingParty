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
    
    [HttpGet("rooms")]
    public async Task<IActionResult> GetAllRooms()
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
    [HttpGet("roomsPage")]
    public async Task<IActionResult> GetAllRoomsWithPaging(int page, int pageSize)
    {
        var response = await _roomService.GetRoomsWithPaging(page, pageSize);
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