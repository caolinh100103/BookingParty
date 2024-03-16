using BusinessLogicLayer.Interfaces;
using Microsoft.AspNetCore.Authorization;
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
    [Authorize(Roles = "Party Host")]
    public async Task<IActionResult> CreateRoom([FromForm] RoomCreatedDTO roomDto)
    {
        var checkRoom = await _roomService.CreateRoom(roomDto);
        return Ok(checkRoom);
    }

    [HttpPut("disabelroom/{roomId}")]
    [Authorize(Roles = "Admin, Party Host")]
    public async Task<IActionResult> DisableRoom(int roomId)
    {
        var result = await _roomService.DisableRoom(roomId);
        if (result.Data)
        {
            return Ok(result);
        }

        return BadRequest();
    }

    [HttpPost("searchAvailableRoom")]
    public async Task<IActionResult> SearchAvailableRoom([FromBody] SearchAvalableRoomDTO searchAvalableRoomDto)
    {
        var result = await _roomService.FindAvailableRoomInDateTime(searchAvalableRoomDto);
        return Ok(result);
    }

    [HttpGet("party_host/rooms/{partyHostId}")]
    [Authorize(Roles = "Party Host")]
    public async Task<IActionResult> GetRoomsByPartyHostId(int partyHostId)
    {
        var result = await _roomService.GetAllRoomsByPartyHostId(partyHostId);
        return Ok(result);
    }

    [HttpPut("room/{roomId}")]
    public async Task<IActionResult> RoomUpdate(int roomId, [FromForm] RoomUpdatedDTO roomUpdated)
    {
        var result = await _roomService.UpdateRoom(roomId, roomUpdated);
        return Ok(result);
    }
    [HttpPost("search_room")]
    public async Task<IActionResult> SearchAllService([FromBody] string searchItem)
    {
        var result = await _roomService.SearchRoom(searchItem);
        return Ok(result);
    }
}