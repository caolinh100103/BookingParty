using BusinessLogicLayer.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Model.DTO;

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
    [HttpPost]
    public async Task<IActionResult> CreateRoom([FromForm] RoomCreatedDTO roomDto)
    {
        var checkRoom = await _roomService.CreateRoom(roomDto);
        return Ok(checkRoom);
    }

    [HttpDelete]
    public async Task<IActionResult> DisableRoom([FromBody] int roomId)
    {
        var result = await _roomService.DisableRoom(roomId);
        if (result.Data)
        {
            return Ok(result);
        }

        return BadRequest();
    }
}