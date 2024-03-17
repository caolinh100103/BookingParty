using System.Collections;
using BusinessLogicLayer.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Model.DTO;

namespace BookingPartyproject.Controllers;
[Route("api/[controller]")]
[ApiController]
public class NotificationController : ControllerBase
{
    private readonly INotificationService _notificationService;
    public NotificationController(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }
    [HttpGet("{userid}")]
    [Authorize(Roles = "Customer,Party Host,Admin")]
    [ProducesResponseType(200, Type = typeof(ResultDTO<ICollection<NotificationResponseDTO>>))]
    public async Task<IActionResult> GetNotificationByUserId(int userid)
    {
        var response = await _notificationService.getNotiByid(userid);
        return Ok(response);
    }
    
    // [HttpGet("{userId}/{notiId}")]
    // [ProducesResponseType(200, Type = typeof(ResultDTO<ICollection<NotificationResponseDTO>>))]
    // public async Task<IActionResult> GetANotificationById(int userId, int notiId)
    // {
    //     
    //     return Ok(response);
    // }
}

